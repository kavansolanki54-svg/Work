using System;
using Android.Content;
using AndroidX.Work;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.ForegroundServices;
using DailyTaskSheet.App.Workers;

namespace DailyTaskSheet.App.Managers
{
    /// <summary>
    /// Manages the scheduling and cancellation of background sync via WorkManager.
    /// Provides methods for periodic, one-time, and foreground service sync triggers.
    /// </summary>
    public static class SyncManager
    {
        private const string Tag = "SyncManager";

        /// <summary>
        /// Schedules a periodic background sync using WorkManager.
        /// Replaces any existing schedule with the new interval.
        /// </summary>
        /// <param name="context">Android context.</param>
        /// <param name="intervalMinutes">Sync interval in minutes (minimum 15 for WorkManager).</param>
        public static void SchedulePeriodicSync(Context context, int intervalMinutes)
        {
            try
            {
                // WorkManager minimum interval is 15 minutes
                int effectiveInterval = Math.Max(intervalMinutes, 15);

                var constraints = new Constraints.Builder()
                    .SetRequiredNetworkType(NetworkType.Connected)
                    .SetRequiresBatteryNotLow(true)
                    .Build();

                var workRequest = PeriodicWorkRequest.Builder
                    .From<BackgroundSyncWorker>(TimeSpan.FromMinutes(effectiveInterval))
                    .SetConstraints(constraints)
                    .SetBackoffCriteria(BackoffPolicy.Exponential,
                        TimeSpan.FromSeconds(AppConstants.InitialBackoffSeconds))
                    .AddTag(AppConstants.SyncWorkTag)
                    .Build();

                WorkManager.GetInstance(context).EnqueueUniquePeriodicWork(
                    AppConstants.PeriodicSyncWorkName,
                    ExistingPeriodicWorkPolicy.Update,
                    workRequest);

                Android.Util.Log.Info(Tag, $"Periodic sync scheduled every {effectiveInterval} minutes.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Failed to schedule periodic sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Schedules a one-time immediate sync using WorkManager.
        /// Used for manual sync triggers and reconnection syncs.
        /// </summary>
        /// <param name="context">Android context.</param>
        public static void ScheduleOneTimeSync(Context context)
        {
            try
            {
                var constraints = new Constraints.Builder()
                    .SetRequiredNetworkType(NetworkType.Connected)
                    .Build();

                var workRequest = OneTimeWorkRequest.Builder
                    .From<BackgroundSyncWorker>()
                    .SetConstraints(constraints)
                    .AddTag(AppConstants.SyncWorkTag)
                    .Build();

                WorkManager.GetInstance(context).EnqueueUniqueWork(
                    AppConstants.OneTimeSyncWorkName,
                    ExistingWorkPolicy.Replace,
                    workRequest);

                Android.Util.Log.Info(Tag, "One-time sync scheduled.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Failed to schedule one-time sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Starts the foreground sync service for immediate execution with a persistent notification.
        /// </summary>
        /// <param name="context">Android context.</param>
        public static void StartForegroundSync(Context context)
        {
            try
            {
                var intent = new Intent(context, typeof(SyncForegroundService));
                intent.SetAction(AppConstants.ActionManualSync);

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    context.StartForegroundService(intent);
                }
                else
                {
                    context.StartService(intent);
                }

                Android.Util.Log.Info(Tag, "Foreground sync service started.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Failed to start foreground sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the foreground sync service.
        /// </summary>
        /// <param name="context">Android context.</param>
        public static void StopForegroundSync(Context context)
        {
            try
            {
                var intent = new Intent(context, typeof(SyncForegroundService));
                intent.SetAction(AppConstants.ActionStopService);
                context.StartService(intent);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Failed to stop foreground sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels all scheduled sync work.
        /// </summary>
        /// <param name="context">Android context.</param>
        public static void CancelAllSync(Context context)
        {
            try
            {
                WorkManager.GetInstance(context).CancelAllWorkByTag(AppConstants.SyncWorkTag);
                StopForegroundSync(context);
                Android.Util.Log.Info(Tag, "All sync work cancelled.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Failed to cancel sync: {ex.Message}");
            }
        }

        /// <summary>
        /// Initializes WorkManager with custom configuration.
        /// Must be called in the Application.OnCreate() before any work is enqueued.
        /// </summary>
        /// <param name="context">Android context.</param>
        public static void InitializeWorkManager(Context context)
        {
            try
            {
                var config = new Configuration.Builder()
                    .SetMinimumLoggingLevel((int)Android.Util.LogPriority.Info)
                    .Build();

                WorkManager.Initialize(context, config);
                Android.Util.Log.Info(Tag, "WorkManager initialized.");
            }
            catch (Java.Lang.IllegalStateException)
            {
                // WorkManager already initialized (this is OK)
                Android.Util.Log.Debug(Tag, "WorkManager was already initialized.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"WorkManager initialization error: {ex.Message}");
            }
        }
    }
}
