using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.ForegroundServices
{
    /// <summary>
    /// Android foreground service that runs call log synchronization with a persistent notification.
    /// Used as a fallback when WorkManager cannot guarantee execution (e.g., Doze mode).
    /// Declared with foregroundServiceType="dataSync" in the manifest for Android 14+ compatibility.
    /// </summary>
    [Service(
        Name = "com.dailytasksheet.android.ForegroundServices.SyncForegroundService",
        Enabled = true,
        Exported = false,
        ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
    public class SyncForegroundService : Service
    {
        private const string Tag = "SyncForegroundService";
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isRunning;

        /// <inheritdoc />
        public override IBinder? OnBind(Intent? intent) => null;

        /// <inheritdoc />
        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            // Handle stop action
            if (intent?.Action == AppConstants.ActionStopService)
            {
                Android.Util.Log.Info(Tag, "Stop action received. Stopping service.");
                StopSelf();
                return StartCommandResult.NotSticky;
            }

            // Start foreground with notification
            var app = (App?)Application;
            var notificationService = app?.GetService<INotificationService>();

            if (notificationService != null)
            {
                var (id, notification) = notificationService.GetForegroundServiceNotification();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    StartForeground(id, notification, Android.Content.PM.ForegroundService.TypeDataSync);
                }
                else
                {
                    StartForeground(id, notification);
                }
            }

            // Start sync if not already running
            if (!_isRunning)
            {
                _isRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => ExecuteSyncLoop(_cancellationTokenSource.Token));
            }

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Runs the sync loop on a background thread.
        /// Executes one sync cycle and stops the service.
        /// </summary>
        private async Task ExecuteSyncLoop(CancellationToken cancellationToken)
        {
            try
            {
                Android.Util.Log.Info(Tag, "Foreground sync starting...");

                var app = (App?)Application;
                var syncService = app?.GetService<ISyncService>();
                var logger = app?.GetService<ILoggerService>();

                if (syncService == null)
                {
                    Android.Util.Log.Error(Tag, "SyncService not available.");
                    return;
                }

                var result = await syncService.ExecuteSyncAsync("ForegroundService", cancellationToken);
                logger?.Info(Tag, $"Foreground sync completed: {result.Message}");

                // Also retry any failed syncs
                await syncService.RetryFailedSyncsAsync(cancellationToken);
            }
            catch (global::System.OperationCanceledException)
            {
                Android.Util.Log.Info(Tag, "Foreground sync cancelled.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Foreground sync error: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
                StopSelf();
            }
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            Android.Util.Log.Info(Tag, "Service being destroyed.");
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            base.OnDestroy();
        }
    }
}
