namespace Backsplashed.Interop.Tasks
{
    using System;
    using Windows.ApplicationModel.Background;
    using Windows.System;
    using Windows.UI.Notifications;

    public class OpenUnsplashLink : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

            if (details != null)
            {
                var arguments = details.Argument; // button argument
                var userInput = details.UserInput;
                var selection = userInput["selection"]; // dropdown value

                // process button
                
                
                // The URI to launch
                // TODO get URI from toast
                // https://docs.microsoft.com/en-us/windows/uwp/launch-resume/launch-default-app
                var uriBing = new Uri(@"http://www.bing.com");

                // Launch the URI
                var success = await Launcher.LaunchUriAsync(uriBing);

                if (success)
                {
                    // URI launched
                }
                else
                {
                    // URI launch failed
                }
                
            }
        }
    }
}