namespace Backsplashed.DependencyInjection
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using Windows.ApplicationModel;
    using Windows.Storage;
    using Core.Abstractions;
    using Interop.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SecureStore.Contrib.Configuration;
    using SelfHost;
    using UI.Embedded;
    using static Interop.Extensions.UwpPackageDetection;

    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureHost(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    var args = Environment.GetCommandLineArgs();

                    if (IsRunningAsUwp)
                    {
                        var basePath = Path.Combine(Package.Current.InstalledLocation.Path, nameof(Backsplashed));
                        configurationBuilder.SetBasePath(basePath);
                    }

                    configurationBuilder
                        .AddJsonFile("backsplashed.json", false, false)
                        .AddSecureStoreFile("backsplashed.secrets.json"
                            , "secrets.key"
                            , KeyType.File, false, false)
                        .AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(provider =>
                    {
                        var localAppDataPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            nameof(Backsplashed)); // C:\Users\\AppData\Local\Backsplashed

                        var storagePath = IsRunningAsUwp
                            ? ApplicationData.Current.LocalFolder.Path // C:\Users\\AppData\Local\Packages\\LocalState
                            : localAppDataPath;

                        var cachePath = IsRunningAsUwp
                            ? ApplicationData.Current.LocalCacheFolder
                                .Path // C:\Users\\AppData\Local\Packages\\LocalCache
                            : localAppDataPath;

                        if (!IsRunningAsUwp)
                        {
                            Directory.CreateDirectory(storagePath);
                        }
                        
                        return new ApplicationOptions
                        {
                            LocalStoragePath = storagePath,
                            LocalCachePath = cachePath
                        };
                    });

                    services.AddDbContext<BacksplashedContext>((provider, optionsBuilder) =>
                    {
                        var options = provider.GetRequiredService<ApplicationOptions>();
                        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                        var dbpath = Path.Combine(options.LocalStoragePath, "backsplashed.db");

                        optionsBuilder.UseSqlite($"Data Source={dbpath}")
                            .EnableSensitiveDataLogging()
                            .ConfigureWarnings(warnings =>
                                warnings.Default(WarningBehavior.Log)
                                    .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning))
                            .UseLoggerFactory(loggerFactory);
                    });

                    services.AddBackgroundWebHost(webHostBuilder => webHostBuilder.ConfigureWebHost(),
                        (provider, host) =>
                        {
                            // get the port from the web host and update the application options
                            var options = provider.GetRequiredService<ApplicationOptions>();
                            var addressFeature = host.ServerFeatures.Get<IServerAddressesFeature>();
                            var port = Regex.Match(addressFeature.Addresses.First(),
                                @"(https?:\/\/.*):(\d*)").Groups[2].Value;
                            options.HostPort = int.Parse(port);
                        });

                    services.AddInteropServices(options =>
                    {
                        context.Configuration.GetSection("Unsplash").Bind(options);
                    });

                    services.AddSingleton<MainWindow>();
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder
                        .AddDebug()
                        .SetMinimumLevel(LogLevel.Debug);
                });
        }

        public static IWebHostBuilder ConfigureWebHost(this IWebHostBuilder builder)
        {
            return builder.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 0,
                        listenOptions => listenOptions.UseHttps());
                })
                .UseStartup<StaticServerStartup>();
        }
    }
}