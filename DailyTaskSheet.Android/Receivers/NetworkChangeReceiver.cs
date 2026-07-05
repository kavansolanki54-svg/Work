using System;
using Android.App;
using Android.Content;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Managers;

namespace DailyTaskSheet.App.Receivers
{
    /// <summary>
    /// Broadcast receiver for network connectivity changes.
    /// Triggers immediate sync when the device reconnects after being offline.
    /// </summary>
    [BroadcastReceiver(
        Name = "com.dailytasksheet.android.Receivers.NetworkChangeReceiver",
        Enabled = true,
        Exported = false)]
    [IntentFilter(new[] { "android.net.conn.CONNECTIVITY_CHANGE" })]
    public class NetworkChangeReceiver : BroadcastReceiver
    {
        private const string Tag = "NetworkChangeReceiver";

        /// <inheritdoc />
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null) return;

            try
            {
                var app = context.ApplicationContext as App;
                if (app == null) return;

                var networkService = app.GetService<INetworkService>();
                var authService = app.GetService<IAuthenticationService>();
                var logger = app.GetService<ILoggerService>();

                if (networkService == null) return;

                // Update network status
                if (networkService is Services.NetworkService ns)
                {
                    ns.UpdateCurrentStatus();
                }

                // If reconnected and authenticated, trigger a one-time sync
                if (networkService.IsConnected && authService?.IsAuthenticated == true)
                {
                    logger?.Info(Tag, "Network reconnected. Scheduling one-time sync.");
                    SyncManager.ScheduleOneTimeSync(context);
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Tag, $"Network receiver error: {ex.Message}");
            }
        }
    }
}
