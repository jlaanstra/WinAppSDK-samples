using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Threading;
using Windows.ApplicationModel.Activation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinAppSDKApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private readonly SynchronizationContext syncContext;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.syncContext = SynchronizationContext.Current;

            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            AppInstance.GetCurrent().Activated += (sender, e) =>
            {
                // Use the synchronous option to ensure AppActivationArguments stays alive.
                // Once the Activated event returns the other instance will close and so 
                // will the AppActivationArguments object.
                this.syncContext.Send(
                    state =>
                    {
                        AppActivationArguments arguments = (AppActivationArguments)state;
                        this.OnActivated(arguments, ApplicationExecutionState.Running);
                    },
                    e);
            };

            var e = AppInstance.GetCurrent().GetActivatedEventArgs();
            this.OnActivated(e, ApplicationExecutionState.NotRunning);
        }

        private void OnActivated(AppActivationArguments args, ApplicationExecutionState previousState)
        {
            if (m_window == null)
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
        }

        private Window m_window;
    }
}
