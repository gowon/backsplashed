namespace Backsplashed
{
    using System;
    using System.IO;
    using System.Windows;
    using Core.Abstractions;
    using Core.Extensions;
    using Interop.Services;
    using Microsoft.Web.WebView2.Core;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WebMessageBinder _mediatorBinder;
        private readonly ApplicationOptions _options;

        public MainWindow(ApplicationOptions options, WebMessageBinder mediatorBinder)
        {
            InitializeComponent();

            _options = options ?? throw new NullReferenceException(nameof(options));
            _mediatorBinder = mediatorBinder ?? throw new NullReferenceException(nameof(options));

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            var options = new CoreWebView2EnvironmentOptions
            {
                AdditionalBrowserArguments = "--allow-insecure-localhost"
            };

            var userDataFolder = Path.Combine(_options.LocalCachePath, "WebView2_Cache");
            var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);

            // Initialize the WebView
            await WebView.EnsureCoreWebView2Async(environment);

            WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            WebView.CoreWebView2.AddHostObjectToScript("backsplashedInterop", _mediatorBinder);

            // Add a script to run when a page is loading
            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(GetType().Assembly
                .ReadResourceAsString("d3-dispatch.js"));
            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(GetType().Assembly
                .ReadResourceAsString("djvi.js"));
            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(GetType().Assembly
                .ReadResourceAsString("djv.js"));
            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(GetType().Assembly
                .ReadResourceAsString("backsplashed.interop.js"));
            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(GetType().Assembly
                .ReadResourceAsString("uuidv4.min.js"));
            
            WebView.Source = _options.HostAddress;
        }

        public async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var json = args.WebMessageAsJson;
            string response = null;

            try
            {
                response = await _mediatorBinder.ProcessInteropCommand(json);
            }
            catch
            {
                // TODO should send an error message with the exception
            }
            finally
            {
                if (sender is CoreWebView2 webView2 && !string.IsNullOrWhiteSpace(response))
                {
                    webView2.PostWebMessageAsJson(response);
                }
            }
        }

        private void fileDevToolsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WebView.CoreWebView2.OpenDevToolsWindow();
        }

        private void fileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}