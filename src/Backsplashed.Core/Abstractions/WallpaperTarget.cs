namespace Backsplashed.Core.Abstractions
{
    using System.ComponentModel;

    public enum WallpaperTarget
    {
        Both,
        Desktop,

        [Description("Lock Screen")] LockScreen
        //Separate
    }
}