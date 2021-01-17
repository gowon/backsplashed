namespace Backsplashed.Core.Abstractions
{
    using System.ComponentModel;

    public enum MonitorResolutionPreset
    {
        Custom,
        [Description("Full HD (1920x1080)")] FullHD,
        [Description("DCI 2K (2048x1080)")] DCI2K,
        [Description("Wide UXGA (1920x1200)")] WideUXGA,
        [Description("Quad HD (2560x1440)")] QuadHD,
        [Description("3K (3000x2000)")] Resolution3K
    }
}