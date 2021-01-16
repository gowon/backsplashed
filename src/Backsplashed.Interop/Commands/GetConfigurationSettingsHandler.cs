namespace Backsplashed.Interop.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Abstractions;
    using MediatR;

    public class GetConfigurationSettingsHandler : IRequestHandler<GetConfigurationSettings, BacksplashedSettings>
    {
        private readonly BacksplashedContext _context;

        public GetConfigurationSettingsHandler(BacksplashedContext context)
        {
            _context = context;
        }

        public async Task<BacksplashedSettings> Handle(GetConfigurationSettings request,
            CancellationToken cancellationToken)
        {
            return await _context.GetBacksplashedSettingsAsync(cancellationToken);
        }
    }
}