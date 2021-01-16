namespace Backsplashed.Core.Abstractions
{
    using System;
    using System.Text.RegularExpressions;

    public static class MonitorResolution
    {
        internal static readonly Regex MonitorResolutionRegex = new Regex(@"(\d+)x(\d+)");

        // https://en.wikipedia.org/wiki/List_of_common_resolutions
        public static readonly string FullHD = "1920x1080";
        public static readonly string DCI2K = "2048x1080";
        public static readonly string WideUXGA = "1920x1200";
        public static readonly string QuadHD = "2560x1440";
        public static readonly string Resolution3K = "3000x2000";

        public static (int height, int width) ToMonitorResolution(this string value)
        {
            var match = MonitorResolutionRegex.Match(value);

            if (!match.Success)
            {
                throw new InvalidOperationException();
            }

            var height = Convert.ToInt32(match.Groups[1].Value);
            var width = Convert.ToInt32(match.Groups[1].Value);
            return (height, width);
        }
    }
}