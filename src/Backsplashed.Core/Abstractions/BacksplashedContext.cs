namespace Backsplashed.Core.Abstractions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class BacksplashedContext : DbContext
    {
        public DbSet<ConfigurationSetting> Settings { get; set; }
        public DbSet<HistoricalPhotoInfo> PhotoHistory { get; set; }
        public DbSet<CachedPhotoInfo> PhotoCache { get; set; }

        public BacksplashedContext(DbContextOptions<BacksplashedContext> options)
            : base(options)
        {
        }

        public async Task Initialize(CancellationToken cancellationToken = default)
        {
            await Database.EnsureCreatedAsync(cancellationToken);
            var count = await Settings.CountAsync(cancellationToken);
            if (count == 0)
            {
                var settings = new BacksplashedSettings
                {
                    Terms = "water,forest,mountain"
                };
                
                await UpdateBacksplashedSettingsAsync(settings, cancellationToken);
            }
        }

        public async Task<BacksplashedSettings> GetBacksplashedSettingsAsync(
            CancellationToken cancellationToken = default)
        {
            var backsplashedSettings = new BacksplashedSettings();
            foreach (var prop in backsplashedSettings.GetType().GetProperties())
            {
                var setting = await Settings.FindAsync(new object[] {prop.Name}, cancellationToken);
                if (setting == null)
                {
                    continue;
                }

                var value = JsonConvert.DeserializeObject(setting.Value, Type.GetType(setting.Type)!);
                prop.SetValue(backsplashedSettings, value);
            }

            return backsplashedSettings;
        }

        public async Task UpdateBacksplashedSettingsAsync(BacksplashedSettings settings,
            CancellationToken cancellationToken = default)
        {
            await Database.ExecuteSqlRawAsync("DELETE FROM Settings", cancellationToken);

            foreach (var prop in settings.GetType().GetProperties())
            {
                await Settings.AddAsync(new ConfigurationSetting
                {
                    Key = prop.Name,
                    Type = prop.PropertyType.FullName,
                    Value = JsonConvert.SerializeObject(prop.GetValue(settings, null))
                }, cancellationToken);
            }

            await SaveChangesAsync(cancellationToken);
        }
    }
}