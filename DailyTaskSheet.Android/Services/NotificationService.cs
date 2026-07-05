using System;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Manages Android notifications for synchronization progress, completion, errors, and boot events.
    /// Creates notification channels on Android 8+ and provides pre-built notification templates.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly Context _context;
        private readonly NotificationManager _notificationManager;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="NotificationService"/>.
        /// </summary>
        public NotificationService(Context context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
            _notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;
        }

        /// <inheritdoc />
        public void CreateNotificationChannels()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            try
            {
                // Sync channel (progress, completion)
                var syncChannel = new NotificationChannel(
                    AppConstants.SyncNotificationChannelId,
                    AppConstants.SyncNotificationChannelName,
                    NotificationImportance.Low)
                {
                    Description = "Notifications for call log synchronization progress and results."
                };
                _notificationManager.CreateNotificationChannel(syncChannel);

                // Error channel (failures, retries)
                var errorChannel = new NotificationChannel(
                    AppConstants.ErrorNotificationChannelId,
                    AppConstants.ErrorNotificationChannelName,
                    NotificationImportance.High)
                {
                    Description = "Notifications for synchronization errors and failures."
                };
                errorChannel.EnableVibration(true);
                _notificationManager.CreateNotificationChannel(errorChannel);

                // General channel (boot, general info)
                var generalChannel = new NotificationChannel(
                    AppConstants.GeneralNotificationChannelId,
                    AppConstants.GeneralNotificationChannelName,
                    NotificationImportance.Default)
                {
                    Description = "General application notifications."
                };
                _notificationManager.CreateNotificationChannel(generalChannel);

                _logger.Info("NotificationService", "Notification channels created.");
            }
            catch (Exception ex)
            {
                _logger.Error("NotificationService", "Failed to create notification channels", ex);
            }
        }

        /// <inheritdoc />
        public void ShowSyncProgressNotification(int progress, int total)
        {
            try
            {
                var builder = new NotificationCompat.Builder(_context, AppConstants.SyncNotificationChannelId)
                    .SetContentTitle("Synchronizing Call Logs")
                    .SetContentText($"Uploading {progress} of {total} records...")
                    .SetSmallIcon(Android.Resource.Drawable.IcPopupSync)
                    .SetOngoing(true)
                    .SetProgress(total, progress, false)
                    .SetPriority(NotificationCompat.PriorityLow);

                _notificationManager.Notify(AppConstants.SyncProgressNotificationId, builder.Build());
            }
            catch (Exception ex)
            {
                _logger.Warning("NotificationService", $"Failed to show progress notification: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void ShowSyncCompleteNotification(int uploaded, int duplicates)
        {
            try
            {
                CancelNotification(AppConstants.SyncProgressNotificationId);

                var builder = new NotificationCompat.Builder(_context, AppConstants.SyncNotificationChannelId)
                    .SetContentTitle("Synchronization Complete")
                    .SetContentText($"Uploaded: {uploaded} | Duplicates: {duplicates}")
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                    .SetAutoCancel(true)
                    .SetPriority(NotificationCompat.PriorityDefault);

                _notificationManager.Notify(AppConstants.SyncCompleteNotificationId, builder.Build());
            }
            catch (Exception ex)
            {
                _logger.Warning("NotificationService", $"Failed to show complete notification: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void ShowSyncFailedNotification(string errorMessage)
        {
            try
            {
                CancelNotification(AppConstants.SyncProgressNotificationId);

                var builder = new NotificationCompat.Builder(_context, AppConstants.ErrorNotificationChannelId)
                    .SetContentTitle("Synchronization Failed")
                    .SetContentText(errorMessage)
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogAlert)
                    .SetAutoCancel(true)
                    .SetPriority(NotificationCompat.PriorityHigh);

                _notificationManager.Notify(AppConstants.SyncFailedNotificationId, builder.Build());
            }
            catch (Exception ex)
            {
                _logger.Warning("NotificationService", $"Failed to show error notification: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void ShowBootCompletedNotification()
        {
            try
            {
                var builder = new NotificationCompat.Builder(_context, AppConstants.GeneralNotificationChannelId)
                    .SetContentTitle("Daily Task Sheet")
                    .SetContentText("Background sync service restarted after boot.")
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                    .SetAutoCancel(true)
                    .SetPriority(NotificationCompat.PriorityDefault);

                _notificationManager.Notify(AppConstants.BootCompletedNotificationId, builder.Build());
            }
            catch (Exception ex)
            {
                _logger.Warning("NotificationService", $"Failed to show boot notification: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void ShowRetryNotification(int retryCount, int maxRetries)
        {
            try
            {
                var builder = new NotificationCompat.Builder(_context, AppConstants.SyncNotificationChannelId)
                    .SetContentTitle("Retrying Synchronization")
                    .SetContentText($"Retry attempt {retryCount} of {maxRetries}")
                    .SetSmallIcon(Android.Resource.Drawable.IcPopupSync)
                    .SetOngoing(true)
                    .SetPriority(NotificationCompat.PriorityLow);

                _notificationManager.Notify(AppConstants.SyncProgressNotificationId, builder.Build());
            }
            catch (Exception ex)
            {
                _logger.Warning("NotificationService", $"Failed to show retry notification: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public (int Id, Notification Notification) GetForegroundServiceNotification()
        {
            var builder = new NotificationCompat.Builder(_context, AppConstants.SyncNotificationChannelId)
                .SetContentTitle("Daily Task Sheet")
                .SetContentText("Call log synchronization is running in the background.")
                .SetSmallIcon(Android.Resource.Drawable.IcPopupSync)
                .SetOngoing(true)
                .SetPriority(NotificationCompat.PriorityLow)
                .SetCategory(Notification.CategoryService);

            return (AppConstants.ForegroundServiceNotificationId, builder.Build());
        }

        /// <inheritdoc />
        public void CancelAllNotifications()
        {
            _notificationManager.CancelAll();
        }

        /// <inheritdoc />
        public void CancelNotification(int notificationId)
        {
            _notificationManager.Cancel(notificationId);
        }
    }
}
