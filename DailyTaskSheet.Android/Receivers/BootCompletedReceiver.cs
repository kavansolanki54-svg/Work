using System;
using Android.App;
using Android.Content;
using Android.OS;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Managers;

namespace DailyTaskSheet.App.Receivers
{
    /// <summary>
    /// Broadcast receiver that triggers on device boot to restart background sync services.
    /// Listens for BOOT_COMPLETED, QUICKBOOT_POWERON, and LOCKED_BOOT_COMPLETED intents.
    /// </summary>
    [BroadcastReceiver(
        Name = "com.dailytasksheet.android.Receivers.BootCompletedReceiver",
        Enabled = true,
        Exported = true,
        DirectBootAware = true)]
    [IntentFilter(new[] {
        Intent.ActionBootCompleted,
        "android.intent.action.QUICKBOOT_POWERON",
        Intent.ActionLockedBootCompleted
    })]
    public class BootCompletedReceiver : BroadcastReceiver
    {
        private const string Tag = "BootReceiver";

        /// <inheritdoc />
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null || intent == null) return;

            string action = intent.Action ?? string.Empty;
            Android.Util.Log.Info(Tag, $"Boot receiver triggered: {action}");

            try
            {
                var app = context.ApplicationContext as App;
                if (app == null)
                {
                    Android.Util.Log.Error(Tag, "App context not available.");
                    return;
                }

                var logger = app.GetService<ILoggerService>();
                var notificationService = app.GetService<INotificationService>();
                var authService = app.GetService<IAuthenticationService>();
                var preferenceService = app.GetService<IPreferenceService>();

                logger?.Info(Tag, "Device boot detected. Restoring sync schedule...");

                // Only restart if user is authenticated and background sync is enabled
                if (authService != null && authService.IsAuthenticated)
                {
                    bool bgSyncEnabled = preferenceService?.GetBool(
                        AppConstants.PrefKeyBackgroundSyncEnabled, true) ?? true;

                    if (bgSyncEnabled)
                    {
                        int intervalMinutes = preferenceService?.GetInt(
                            AppConstants.PrefKeySyncInterval,
                            AppConstants.DefaultSyncIntervalMinutes) ?? AppConstants.DefaultSyncIntervalMinutes;

                        // Schedule WorkManager periodic sync
                        SyncManager.SchedulePeriodicSync(context, intervalMinutes);

                        logger?.Info(Tag, $"Background sync rescheduled (interval: {intervalMinutes} min).");
                        notificationService?.ShowBootCompletedNotification();
                    }
                    else
                    {
                        logger?.Info(Tag, "Background sync is disabled in settings.");
                    }
                }
                else
                {
                    logger?.Info(Tag, "User not authenticated. Sync not restarted.");
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Boot receiver error: {ex.Message}");
            }
        }
    }
}
