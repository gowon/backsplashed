namespace Backsplashed.Core.Abstractions
{
    using System;

    public class ApplicationOptions
    {
        public int HostPort { get; set; }

        public Uri HostAddress => new($"https://localhost:{HostPort}", UriKind.Absolute);
        public string LocalStoragePath { get; set; }
        public string LocalCachePath { get; set; }
    }
}