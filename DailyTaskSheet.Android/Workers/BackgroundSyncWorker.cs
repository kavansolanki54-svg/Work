using System;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Workers
{
    /// <summary>
    /// AndroidX WorkManager periodic worker that executes call log synchronization
    /// in the background. Scheduled to run at configurable intervals (default 15 min).
    /// WorkManager handles constraints (network, battery), retries, and OS-level scheduling.
    /// </summary>
    [Register("com.dailytasksheet.android.workers.BackgroundSyncWorker")]
    public class BackgroundSyncWorker : Worker
    {
        private const string Tag = "BackgroundSyncWorker";

        /// <summary>
        /// Initializes a new instance of <see cref="BackgroundSyncWorker"/>.
        /// Required by AndroidX WorkManager for reflection-based instantiation.
        /// </summary>
        public BackgroundSyncWorker(Context context, WorkerParameters workerParams)
            : base(context, workerParams)
        {
        }

        /// <summary>
        /// Executes the background synchronization work.
        /// Called by WorkManager when constraints are met.
        /// </summary>
        /// <returns>Success, Failure, or Retry result.</returns>
        public override Result DoWork()
        {
            try
            {
                Android.Util.Log.Info(Tag, "Background sync worker started.");

                // Get services from the application DI container
                var app = (App?)ApplicationContext;
                if (app == null)
                {
                    Android.Util.Log.Error(Tag, "Application context is null.");
                    return Result.InvokeFailure();
                }

                var syncService = app.GetService<ISyncService>();
                var logger = app.GetService<ILoggerService>();
                var authService = app.GetService<IAuthenticationService>();

                if (syncService == null || authService == null)
                {
                    Android.Util.Log.Error(Tag, "Required services not available.");
                    return Result.InvokeFailure();
                }

                // Check if user is authenticated
                if (!authService.IsAuthenticated)
                {
                    logger?.Info(Tag, "User not authenticated. Skipping background sync.");
                    return Result.InvokeSuccess();
                }

                // Execute sync synchronously (WorkManager provides its own thread)
                var task = syncService.ExecuteSyncAsync("WorkManager");
                task.Wait();

                var result = task.Result;
                if (result.Status == (int)Enums.SyncStatusEnum.Synced ||
                    result.Status == (int)Enums.SyncStatusEnum.Pending)
                {
                    logger?.Info(Tag, $"Background sync completed: {result.Message}");
                    return Result.InvokeSuccess();
                }
                else
                {
                    logger?.Warning(Tag, $"Background sync failed: {result.Message}");
                    return Result.InvokeRetry();
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Background sync worker error: {ex.Message}");
                return Result.InvokeRetry();
            }
        }
    }
}
