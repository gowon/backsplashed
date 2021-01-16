namespace Backsplashed.Interop.Commands
{
    using Core.Abstractions;
    using MediatR;

    public class GetConfigurationSettings : IRequest<BacksplashedSettings>
    {
    }
}