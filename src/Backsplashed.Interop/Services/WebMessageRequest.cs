namespace Backsplashed.Interop.Services
{
    using System.Runtime.InteropServices;

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class WebMessageRequest
    {
        public string Type { get; set; }
        public string JsonData { get; set; }
        public string Callback { get; set; }
    }
}