namespace Backsplashed.Interop.DependencyInjection
{
    using System;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using OASplash.Abstractions;
    using OASplash.DependencyInjection;
    using Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInteropServices(this IServiceCollection services
            , Action<UnsplashOptions> configureOptions = null)
        {
            services.AddSingleton<WebMessageBinder>();
            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);
            services.AddUnsplashClient(configureOptions);

            return services;
        }
    }
}