namespace Backsplashed.Core.Abstractions
{
    using System.ComponentModel.DataAnnotations;

    public class BacksplashedSettings
    {
        public string Terms { get; set; }
        public MonitorResolutionPreset ResolutionPreset { get; set; } = MonitorResolutionPreset.FullHD;
        public MonitorResolution Resolution { get; set; } = MonitorResolution.FullHD;
        public WallpaperTarget Target { get; set; } = WallpaperTarget.Both;
        public AutoUpdateIntervalPreset AutoUpdateIntervalPreset { get; set; } = AutoUpdateIntervalPreset.FifteenMinutes;

        /// <summary>
        ///     The update interval can be set to anything between (15 min, 24 hrs)
        /// </summary>
        [Range(15, 1440)]
        public int AutoUpdateInterval { get; set; } = 15;

        public bool IsAutoUpdateEnabled { get; set; }
        public bool IsNotifyUpdateEnabled { get; set; } = true;
    }
}