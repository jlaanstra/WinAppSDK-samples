using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinAppSDKApp
{
#if DISABLE_XAML_GENERATED_MAIN
    public static class Program
    {
        private static App app;

        // NOTE: can't be async or the STAThread attribute won't be applied.
        // See https://github.com/dotnet/csharplang/issues/97
        [STAThread]
        public static void Main(string[] args)
        {
            global::WinRT.ComWrappersSupport.InitializeComWrappers();

            var mainInstance = AppInstance.FindOrRegisterForKey("main");
            if (mainInstance.IsCurrent)
            {
                global::Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var syncContext = new WinAppSDKApp.DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(syncContext);

                    app = new App();
                });

                mainInstance.UnregisterKey();
            }
            else
            {
                RedirectActivationToAsync(mainInstance, AppInstance.GetCurrent().GetActivatedEventArgs()).Wait();
            }
        }

        private static async Task RedirectActivationToAsync(AppInstance mainInstance, AppActivationArguments activationArgs)
        {
            await TaskEx.YieldToBackground();

            await mainInstance.RedirectActivationToAsync(activationArgs);
        }
    }
#endif
}
