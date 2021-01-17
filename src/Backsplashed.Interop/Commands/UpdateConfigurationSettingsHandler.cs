namespace Backsplashed.Interop.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Abstractions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class UpdateConfigurationSettingsHandler : IRequestHandler<UpdateConfigurationSettings, bool>
    {
        private readonly ILogger<UpdateConfigurationSettingsHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly BacksplashedContext _context;

        public UpdateConfigurationSettingsHandler(ILogger<UpdateConfigurationSettingsHandler> logger,
            IServiceScopeFactory scopeFactory, BacksplashedContext context)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _context = context;
        }

        public async Task<bool> Handle(UpdateConfigurationSettings request, CancellationToken cancellationToken)
        {
            try
            {
                var settings = Convert(request);
                await _context.UpdateBacksplashedSettingsAsync(settings, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while trying to update application settings.");
                return false;
            }
        }

        //public async Task<bool> Handle(UpdateConfigurationSettings request, CancellationToken cancellationToken)
        //{
        //    var settings = Convert(request);

        //    try
        //    {
        //        using (var scope = _scopeFactory.CreateScope())
        //        {
        //            var context = scope.ServiceProvider.GetRequiredService<BacksplashedContext>();
        //            await context.UpdateBacksplashedSettingsAsync(settings, cancellationToken);
        //            return true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error occurred while trying to update application settings.");
        //        return false;
        //    }
        //}

        private BacksplashedSettings Convert(UpdateConfigurationSettings request)
        {
            return new()
            {
                Terms = request.Terms,
                Target = (WallpaperTarget) request.Target,
                Resolution = MonitorResolution.FromString(request.Resolution),
                IsAutoUpdateEnabled = request.IsAutoUpdateEnabled,
                AutoUpdateInterval = request.AutoUpdateInterval,
                IsNotifyUpdateEnabled = request.IsNotifyUpdateEnabled
            };
        }
    }
}