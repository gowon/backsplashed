namespace Backsplashed.Interop.Commands
{
    using System.ComponentModel.DataAnnotations;
    using MediatR;

    public class UpdateConfigurationSettings : IRequest<bool>
    {
        [Required]
        public string Terms { get; set; }

        [Required]
        public string Resolution { get; set; }

        [Required]
        public int Target { get; set; }

        [Required]
        public int AutoUpdateInterval { get; set; }

        [Required]
        public bool IsAutoUpdateEnabled { get; set; }

        [Required]
        public bool IsNotifyUpdateEnabled { get; set; }
    }
}