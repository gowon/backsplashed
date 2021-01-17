namespace Backsplashed.Core.Abstractions
{
    using System.ComponentModel;

    public enum AutoUpdateIntervalPreset
    {
        Custom = 0,
        [Description("15 minutes")] FifteenMinutes = 15,
        [Description("30 minutes")] ThirtyMinutes = 30,
        [Description("1 hour")] OneHour = 60,
        [Description("2 hours")] TwoHours = 120,
        [Description("24 hours")] TwentyFourHours = 1440
    }
}