using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Shared;
using Shared.Utilities;
using SharedNative;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinAppSDKApp.Utilities;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinAppSDKApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private const string ExampleTaskName = "ToastBgTask";
        private const string ExampleTaskEntryPoint = "BackgroundTasks.BackgroundTask";

        private readonly SynchronizationContext syncContext;

        private Window m_window;
        private ClassFactory<InProcBackgroundTask> inProcBackgroundTaskFactory;

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

                RegisterComServer();
                RegisterTasks();
            }
        }

        public async Task OnBackgroundActivatedAsync(IBackgroundTaskInstance instance)
        {
            await TaskEx.YieldToBackground();

            ToastHelper.ShowToast();
        }

        private void RegisterComServer()
        {
            this.inProcBackgroundTaskFactory = new ClassFactory<InProcBackgroundTask>(
                () => new InProcBackgroundTask(),
                new Dictionary<Guid, Func<object, IntPtr>>()
                {
                    { typeof(IBackgroundTask).GUID, obj => MarshalInterface<IBackgroundTask>.FromManaged((IBackgroundTask)obj) },
                });
            // On launch register the BackgroundTask class for OOP COM activation
            ComUtilities.RegisterClass<InProcBackgroundTask>(this.inProcBackgroundTaskFactory);
        }

        private void RegisterTasks()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == ExampleTaskName)
                {
                    task.Value.Unregister(true);
                }
            }


            var builder = new BackgroundTaskBuilder
            {
                Name = ExampleTaskName,
                // Set either the TaskEntryPointClsId for Win32 bg tasks (limited triggers supported)
                // or use the Out-OfProc workaround for others.
                TaskEntryPoint = ExampleTaskEntryPoint,
            };
            // Uncomment this for Win32 bg tasks where the system directly activates the COM server,
            // and update the manifest.
            // See https://docs.microsoft.com/en-us/windows/uwp/launch-resume/create-and-register-a-winmain-background-task
            // builder.SetTaskEntryPointClsid(typeof(InProcBackgroundTask).GUID);
            builder.SetTrigger(new TimeTrigger(15, true));
            builder.Register();
        }
    }
}
