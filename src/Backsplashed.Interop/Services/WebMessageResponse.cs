namespace Backsplashed.Interop.Services
{
    using System.Runtime.InteropServices;

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class WebMessageResponse
    {
        public string Callback { get; set; }
        public string JsonData { get; set; }
    }
}