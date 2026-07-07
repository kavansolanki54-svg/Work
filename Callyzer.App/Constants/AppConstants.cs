namespace Callyzer.App.Constants
{
    /// <summary>
    /// Application-wide constants.
    /// </summary>
    public static class AppConstants
    {
        // ═══════════════════════════════════════════════════════
        // Application Identity
        // ═══════════════════════════════════════════════════════
        public const string AppName = "Callyzer";
        public const string AppVersion = "1.0.0";
        public const string PackageName = "com.callyzer.app";

        // ═══════════════════════════════════════════════════════
        // Database
        // ═══════════════════════════════════════════════════════
        public const string DatabaseName = "callyzer.db";
        public const int DatabaseVersion = 1;

        // ═══════════════════════════════════════════════════════
        // Shared Preferences
        // ═══════════════════════════════════════════════════════
        public const string SecurePrefsName = "callyzer_secure_prefs";
        public const string AuthPrefsName = "callyzer_auth";
        public const string SettingsPrefsName = "callyzer_settings";

        // ═══════════════════════════════════════════════════════
        // Preference Keys
        // ═══════════════════════════════════════════════════════
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
        public const string PrefKeyApiBaseUrl = "pref_api_base_url";
        public const string PrefKeySyncInterval = "pref_sync_interval";
        public const string PrefKeyBackgroundSyncEnabled = "pref_background_sync_enabled";
        public const string PrefKeyWifiOnly = "pref_wifi_only";
        public const string PrefKeyNotificationsEnabled = "pref_notifications_enabled";
        public const string PrefKeyLoggingEnabled = "pref_logging_enabled";
        public const string PrefKeyAutoRetry = "pref_auto_retry";
        public const string PrefKeyDarkMode = "pref_dark_mode";
        public const string PrefKeyDatabaseVersion = "pref_database_version";

        // ═══════════════════════════════════════════════════════
        // Sync Configuration
        // ═══════════════════════════════════════════════════════
        public const int DefaultSyncIntervalMinutes = 15;
        public const int MinSyncIntervalMinutes = 5;
        public const int MaxSyncIntervalMinutes = 60;
        public const int MaxRetryAttempts = 10;
        public const int InitialBackoffSeconds = 30;
        public const int MaxBatchSize = 100;

        // ═══════════════════════════════════════════════════════
        // HTTP Configuration
        // ═══════════════════════════════════════════════════════
        public const int HttpTimeoutSeconds = 30;
        public const int HttpMaxRetries = 3;
        public const string DefaultApiBaseUrl = "https://dts.runasp.net";

        // ═══════════════════════════════════════════════════════
        // Notification Channels (Android-specific)
        // ═══════════════════════════════════════════════════════
        public const string SyncNotificationChannelId = "callyzer_sync_channel";
        public const string SyncNotificationChannelName = "Synchronization";
        public const string ErrorNotificationChannelId = "callyzer_error_channel";
        public const string ErrorNotificationChannelName = "Errors";
        public const string GeneralNotificationChannelId = "callyzer_general_channel";
        public const string GeneralNotificationChannelName = "General";

        // ═══════════════════════════════════════════════════════
        // Notification IDs
        // ═══════════════════════════════════════════════════════
        public const int ForegroundServiceNotificationId = 1001;
        public const int SyncProgressNotificationId = 1002;
        public const int SyncCompleteNotificationId = 1003;
        public const int SyncFailedNotificationId = 1004;
        public const int DailySummaryNotificationId = 1005;

        // ═══════════════════════════════════════════════════════
        // WorkManager (Android) / BGTaskScheduler (iOS)
        // ═══════════════════════════════════════════════════════
        public const string PeriodicSyncWorkName = "callyzer_periodic_sync";
        public const string OneTimeSyncWorkName = "callyzer_onetime_sync";
        public const string SyncWorkTag = "callyzer_sync_tag";
        public const string DailySummaryWorkName = "callyzer_daily_summary";

        // ═══════════════════════════════════════════════════════
        // Analytics
        // ═══════════════════════════════════════════════════════
        public const int DefaultTopContactsLimit = 10;
        public const int DefaultLongestCallsLimit = 10;
        public const int AnalyticsCacheTtlMinutes = 30;
    }
}
