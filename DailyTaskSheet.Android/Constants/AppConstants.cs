using System;

namespace DailyTaskSheet.App.Constants
{
    /// <summary>
    /// Application-wide constants used throughout the DailyTaskSheet application.
    /// Centralizes all magic strings, preference keys, and default values.
    /// </summary>
    public static class AppConstants
    {
        // ══════════════════════════════════════════════════════════════════
        // Application Identity
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Application display name.</summary>
        public const string AppName = "Daily Task Sheet";

        /// <summary>Application version string.</summary>
        public const string AppVersion = "1.0.0";

        /// <summary>Application package identifier.</summary>
        public const string PackageName = "com.dailytasksheet.android";

        // ══════════════════════════════════════════════════════════════════
        // Database
        // ══════════════════════════════════════════════════════════════════

        /// <summary>SQLite database file name.</summary>
        public const string DatabaseName = "dailytasksheet.db";

        /// <summary>Current database schema version for migration support.</summary>
        public const int DatabaseVersion = 1;

        // ══════════════════════════════════════════════════════════════════
        // Shared Preferences
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Name of the secure shared preferences file.</summary>
        public const string SecurePrefsName = "daily_task_sheet_secure_prefs";

        /// <summary>Name of the authentication shared preferences file.</summary>
        public const string AuthPrefsName = "daily_task_sheet_auth";

        /// <summary>Name of the general settings shared preferences file.</summary>
        public const string SettingsPrefsName = "daily_task_sheet_settings";

        // ══════════════════════════════════════════════════════════════════
        // Preference Keys
        // ══════════════════════════════════════════════════════════════════

        public const string PrefKeyAccessToken = "pref_access_token";
        public const string PrefKeyRefreshToken = "pref_refresh_token";
        public const string PrefKeyTokenExpiry = "pref_token_expiry";
        public const string PrefKeyEmployeeId = "pref_employee_id";
        public const string PrefKeyCompanyId = "pref_company_id";
        public const string PrefKeyCompanyName = "pref_company_name";
        public const string PrefKeyUserId = "pref_user_id";
        public const string PrefKeyUserName = "pref_user_name";
        public const string PrefKeyEmail = "pref_email";
        public const string PrefKeyRoleName = "pref_role_name";
        public const string PrefKeyRoleId = "pref_role_id";
        public const string PrefKeyDeviceId = "pref_device_id";
        public const string PrefKeyIsLoggedIn = "pref_is_logged_in";
        public const string PrefKeyLastSyncTime = "pref_last_sync_time";
        public const string PrefKeyLastSyncCallId = "pref_last_sync_call_id";
        public const string PrefKeyCustomRecordingPath = "pref_custom_recording_path";
        public const string PrefKeyApiBaseUrl = "pref_api_base_url";
        public const string PrefKeySyncInterval = "pref_sync_interval";
        public const string PrefKeyBackgroundSyncEnabled = "pref_background_sync_enabled";
        public const string PrefKeyWifiOnly = "pref_wifi_only";
        public const string PrefKeyUseMobileData = "pref_use_mobile_data";
        public const string PrefKeyNotificationsEnabled = "pref_notifications_enabled";
        public const string PrefKeyLoggingEnabled = "pref_logging_enabled";
        public const string PrefKeyAutoRetry = "pref_auto_retry";
        public const string PrefKeyDarkMode = "pref_dark_mode";
        public const string PrefKeyEncryptionKey = "pref_encryption_key";
        public const string PrefKeyDatabaseVersion = "pref_database_version";

        // ══════════════════════════════════════════════════════════════════
        // Sync Configuration
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Default synchronization interval in minutes.</summary>
        public const int DefaultSyncIntervalMinutes = 15;

        /// <summary>Minimum synchronization interval in minutes.</summary>
        public const int MinSyncIntervalMinutes = 5;

        /// <summary>Maximum synchronization interval in minutes.</summary>
        public const int MaxSyncIntervalMinutes = 60;

        /// <summary>Maximum number of retry attempts for failed synchronization.</summary>
        public const int MaxRetryAttempts = 10;

        /// <summary>Initial backoff delay in seconds for exponential retry.</summary>
        public const int InitialBackoffSeconds = 30;

        /// <summary>Maximum number of call logs per sync batch.</summary>
        public const int MaxBatchSize = 100;

        // ══════════════════════════════════════════════════════════════════
        // HTTP Configuration
        // ══════════════════════════════════════════════════════════════════

        /// <summary>HTTP request timeout in seconds.</summary>
        public const int HttpTimeoutSeconds = 30;

        /// <summary>Maximum HTTP retry attempts for transient failures.</summary>
        public const int HttpMaxRetries = 3;

        /// <summary>Default API base URL (user-configurable).</summary>
        public const string DefaultApiBaseUrl = "https://dts.runasp.net";

        // ══════════════════════════════════════════════════════════════════
        // Notification Channels
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Notification channel ID for sync-related notifications.</summary>
        public const string SyncNotificationChannelId = "dts_sync_channel";

        /// <summary>Notification channel name for sync-related notifications.</summary>
        public const string SyncNotificationChannelName = "Synchronization";

        /// <summary>Notification channel ID for error notifications.</summary>
        public const string ErrorNotificationChannelId = "dts_error_channel";

        /// <summary>Notification channel name for error notifications.</summary>
        public const string ErrorNotificationChannelName = "Errors";

        /// <summary>Notification channel ID for general notifications.</summary>
        public const string GeneralNotificationChannelId = "dts_general_channel";

        /// <summary>Notification channel name for general notifications.</summary>
        public const string GeneralNotificationChannelName = "General";

        // ══════════════════════════════════════════════════════════════════
        // Notification IDs
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Notification ID for the foreground sync service.</summary>
        public const int ForegroundServiceNotificationId = 1001;

        /// <summary>Notification ID for sync progress notifications.</summary>
        public const int SyncProgressNotificationId = 1002;

        /// <summary>Notification ID for sync completion notifications.</summary>
        public const int SyncCompleteNotificationId = 1003;

        /// <summary>Notification ID for sync failure notifications.</summary>
        public const int SyncFailedNotificationId = 1004;

        /// <summary>Notification ID for boot completed notifications.</summary>
        public const int BootCompletedNotificationId = 1005;

        // ══════════════════════════════════════════════════════════════════
        // WorkManager
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Unique work name for the periodic sync worker.</summary>
        public const string PeriodicSyncWorkName = "dts_periodic_sync";

        /// <summary>Unique work name for a one-time immediate sync.</summary>
        public const string OneTimeSyncWorkName = "dts_onetime_sync";

        /// <summary>Tag for sync-related WorkManager jobs.</summary>
        public const string SyncWorkTag = "dts_sync_tag";

        // ══════════════════════════════════════════════════════════════════
        // Intent Extras
        // ══════════════════════════════════════════════════════════════════

        /// <summary>Intent extra key indicating the source of a sync trigger.</summary>
        public const string ExtraSyncSource = "extra_sync_source";

        /// <summary>Intent action for manual sync trigger.</summary>
        public const string ActionManualSync = "com.dailytasksheet.android.ACTION_MANUAL_SYNC";

        /// <summary>Intent action to stop the foreground service.</summary>
        public const string ActionStopService = "com.dailytasksheet.android.ACTION_STOP_SERVICE";
    }
}
