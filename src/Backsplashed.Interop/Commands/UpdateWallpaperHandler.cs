namespace Backsplashed.Interop.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Data.Xml.Dom;
    using Windows.Storage;
    using Windows.System.UserProfile;
    using Windows.UI.Notifications;
    using Core.Abstractions;
    using Core.Extensions;
    using Extensions;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OASplash.Client;
    using OASplash.Client.Models;
    using static Extensions.UwpPackageDetection;

    public class UpdateWallpaperHandler : AsyncRequestHandler<UpdateWallpaper>
    {
        private const string ImagesPath = "Images";
        private readonly IUnsplashClient _client;
        private readonly BacksplashedContext _context;
        private readonly ILogger<UpdateWallpaperHandler> _logger;

        public UpdateWallpaperHandler(ILogger<UpdateWallpaperHandler> logger, BacksplashedContext context,
            IUnsplashClient client)
        {
            _logger = logger;
            _context = context;
            _client = client;
        }

        protected override async Task Handle(UpdateWallpaper request, CancellationToken cancellationToken)
        {
            if (!IsRunningAsUwp || !UserProfilePersonalizationSettings.IsSupported())
            {
                _logger.Log(LogLevel.Information, "UserProfilePersonalizationSettings not supported on this device.");
                return;
            }

            var now = DateTime.Now;
            var random = new Random(now.Millisecond);
            var backsplashedSettings = await _context.GetBacksplashedSettingsAsync(cancellationToken);
            var terms = backsplashedSettings.Terms.Split(',');
            var term = terms[random.Next(terms.Length)];
            PhotoInfo info;
            
            // get random photo
            var cached = await _context.PhotoCache.Where(cachedPhoto => cachedPhoto.Term == term).ToListAsync(cancellationToken: cancellationToken);

            if (cached.Any())
            {
                var entry = cached.PopAt(random.Next(cached.Count));
                info = entry;
                var used = new HistoricalPhotoInfo(entry)
                {
                    Timestamp = now
                };
                
                _context.PhotoCache.Remove(entry);
                await _context.PhotoHistory.AddAsync(used, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                var results = new List<CachedPhotoInfo>();
                while (results.Count == 0)
                {
                    var photos = await _client.GetRandomPhotosAsync(
                        query: terms[random.Next(terms.Length)], count: 30, orientation: Orientation.Landscape,
                        cancellationToken: cancellationToken);
                    results = photos.Where(p => p.Width >= backsplashedSettings.Resolution.Width && p.Height >= backsplashedSettings.Resolution.Height)
                        .Select(p => p.ToPhotoInfo(term)).ToList();
                }

                var entry = results.PopAt(random.Next(results.Count));
                info = entry;
                var used = new HistoricalPhotoInfo(entry)
                {
                    Timestamp = now
                };
                
                await _context.PhotoCache.AddRangeAsync(results, cancellationToken);
                await _context.PhotoHistory.AddAsync(used, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            byte[] bytes;
            using (var httpClient = new HttpClient())
            using (var result = await httpClient.GetAsync(info.RawUrl, cancellationToken))
            {
                result.EnsureSuccessStatusCode();
                bytes = await result.Content.ReadAsByteArrayAsync(cancellationToken);
                await _client.TrackPhotoDownloadAsync(info.Id, cancellationToken);
            }

            // save source image to local storage in order to update targets
            await ClearImagesFolderAsync();
            var storageFile = await SaveImageAsLocalStorageFileAsync($"{info.Id}.jpg", bytes);

            // Only images stored in the LocalFolder can be used to apply to the wallpaper
            // It is advised to always randomly generate the file-name given quirks to the
            // TrySetWallpaperImageAsync method.
            // https://docs.microsoft.com/en-us/uwp/api/windows.system.userprofile.userprofilepersonalizationsettings.trysetwallpaperimageasync#remarks
            var settings = UserProfilePersonalizationSettings.Current;

            if (backsplashedSettings.Target == WallpaperTarget.Desktop || backsplashedSettings.Target == WallpaperTarget.Both)
            {
                var success = await settings.TrySetWallpaperImageAsync(storageFile);
                _logger.Log(success ? LogLevel.Information : LogLevel.Warning,
                    success
                        ? "Desktop wallpaper update was successful."
                        : "Desktop wallpaper update was not successful.");
            }

            if (backsplashedSettings.Target == WallpaperTarget.LockScreen || backsplashedSettings.Target == WallpaperTarget.Both)
            {
                var success = await settings.TrySetLockScreenImageAsync(storageFile);
                _logger.Log(success ? LogLevel.Information : LogLevel.Warning,
                    success
                        ? "Lock screen wallpaper update was successful."
                        : "Lock screen wallpaper update was not successful.");
            }

            // send notification toast
            if (backsplashedSettings.IsNotifyUpdateEnabled)
            {
                var toastXml = GetType().Assembly.ReadResourceAsString("BacksplashedToast.xml");
                var xml = new XmlDocument();
                xml.LoadXml(toastXml);
                
                var toastNode = (XmlElement)xml.SelectSingleNode("//toast");
                toastNode.SetAttribute("launch", info.LinkUrl);

                var textNode = (XmlElement)xml.SelectSingleNode("//text[2]");
                textNode.InnerText = $"Photo by {info.Author} on Unsplash";
                
                var imageNode = (XmlElement)xml.SelectSingleNode("//image");
                imageNode.SetAttribute("src", info.ThumbUrl);
                
                var saveButton = (XmlElement)xml.SelectSingleNode("//action[@content='Save']");
                saveButton.SetAttribute("arguments", $"action=saveCurrentWallpaper&amp;photoId={info.Id}");
                
                var toastNotification = new ToastNotification(xml);
                var toastNotifier = ToastNotificationManager.CreateToastNotifier();
                toastNotifier.Show(toastNotification);
            }
        }

        private static async Task ClearImagesFolderAsync()
        {
            // clear old files from folder
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ImagesPath,
                CreationCollisionOption.OpenIfExists);
            var files = await folder.GetFilesAsync();
            foreach(var oldFile in files)
            {
                await oldFile.DeleteAsync(StorageDeleteOption.Default);                
            }
        }
        
        private static async Task<StorageFile> SaveImageAsLocalStorageFileAsync(string fileName, byte[] byteArray)
        {
            var storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ImagesPath,
                CreationCollisionOption.OpenIfExists);
            var sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(sampleFile, byteArray);
            return sampleFile;
        }
    }
}