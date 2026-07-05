namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for managing Android notifications.
    /// Handles notification channels, progress updates, and notification display.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>Creates all required notification channels (must be called on startup).</summary>
        void CreateNotificationChannels();

        /// <summary>Shows a notification that synchronization is in progress.</summary>
        /// <param name="progress">Current progress count.</param>
        /// <param name="total">Total items to sync.</param>
        void ShowSyncProgressNotification(int progress, int total);

        /// <summary>Shows a notification that synchronization completed successfully.</summary>
        /// <param name="uploaded">Number of records uploaded.</param>
        /// <param name="duplicates">Number of duplicates detected.</param>
        void ShowSyncCompleteNotification(int uploaded, int duplicates);

        /// <summary>Shows a notification that synchronization failed.</summary>
        /// <param name="errorMessage">Brief error description.</param>
        void ShowSyncFailedNotification(string errorMessage);

        /// <summary>Shows a notification that the boot receiver has restarted services.</summary>
        void ShowBootCompletedNotification();

        /// <summary>Shows a notification for retry attempts.</summary>
        /// <param name="retryCount">Current retry attempt number.</param>
        /// <param name="maxRetries">Maximum retries allowed.</param>
        void ShowRetryNotification(int retryCount, int maxRetries);

        /// <summary>Gets the foreground service notification for the sync service.</summary>
        /// <returns>A tuple of (notification ID, notification object).</returns>
        (int Id, Android.App.Notification Notification) GetForegroundServiceNotification();

        /// <summary>Cancels all notifications.</summary>
        void CancelAllNotifications();

        /// <summary>Cancels a specific notification by ID.</summary>
        /// <param name="notificationId">The notification ID to cancel.</param>
        void CancelNotification(int notificationId);
    }
}
