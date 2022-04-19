using System;
using Windows.ApplicationModel.Background;

namespace BackgroundTasks
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public const int E_ACCESSDENIED = unchecked((int)0x80070005);

        public BackgroundTask()
        {
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = null;

            try
            {
                deferral = taskInstance.GetDeferral();

                var backgroundTaskClient = new BackgroundTaskClient();
                backgroundTaskClient.Run(taskInstance);
            }
            catch (Exception e) when (e.HResult == E_ACCESSDENIED)
            {
                // Access Denied happens in Connected Standby,
                // where BG tasks can run but the centennial process cannot.
                // Ignore these errors.
            }
            finally
            {
                if (deferral != null)
                {
                    deferral.Complete();
                }
            }
        }
    }
}