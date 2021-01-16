﻿namespace Backsplashed.Interop.Extensions
{
    using System;
    using Windows.ApplicationModel;

    public static class UwpPackageDetection
    {
        private static readonly Lazy<bool> UwpDetectedLazy = new(() =>
        {
            try
            {
                var package = Package.Current;
                return package != null;
            }
            catch
            {
                return false;
            }
        });

        public static bool IsRunningAsUwp => UwpDetectedLazy.Value;
    }
}