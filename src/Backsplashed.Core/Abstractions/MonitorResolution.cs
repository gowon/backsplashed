// ReSharper disable InconsistentNaming

namespace Backsplashed.Core.Abstractions
{
    using System;
    using System.Text.RegularExpressions;

    public struct MonitorResolution
    {
        public MonitorResolution(int height, int width)
        {
            Height = height;
            Width = width;
        }

        public int Height { get; set; }
        public int Width { get; set; }

        public override string ToString()
        {
            return $"({Height}x{Width})";
        }

        private static readonly Regex MonitorResolutionRegex = new(@"(\d+)x(\d+)");

        // https://en.wikipedia.org/wiki/List_of_common_resolutions
        public static readonly MonitorResolution FullHD = new(1920, 1080);
        public static readonly MonitorResolution DCI2K = new(2048, 1080);
        public static readonly MonitorResolution WideUXGA = new(1920, 1200);
        public static readonly MonitorResolution QuadHD = new(2560, 1440);
        public static readonly MonitorResolution Resolution3K = new(3000, 2000);

        public static MonitorResolution FromString(string value)
        {
            var match = MonitorResolutionRegex.Match(value);

            if (!match.Success)
            {
                throw new InvalidOperationException();
            }

            var height = Convert.ToInt32(match.Groups[1].Value);
            var width = Convert.ToInt32(match.Groups[2].Value);
            return new(height, width);
        }
    }
}