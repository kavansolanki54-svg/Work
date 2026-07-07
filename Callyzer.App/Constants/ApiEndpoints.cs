namespace Callyzer.App.Constants
{
    /// <summary>
    /// Centralizes all REST API endpoint paths.
    /// </summary>
    public static class ApiEndpoints
    {
        // ═══════════════════════════════════════════════════════
        // Authentication
        // ═══════════════════════════════════════════════════════
        public const string Login = "/api/Auth/login";
        public const string RefreshToken = "/api/Auth/RefreshToken";

        // ═══════════════════════════════════════════════════════
        // Call Log Synchronization
        // ═══════════════════════════════════════════════════════
        public const string SyncCallLogs = "/api/CallAnalytics/sync";
        public const string UploadNativeRecording = "/api/CallLogs/UploadNativeRecording";
        public const string SyncHistory = "/api/CallLogs/History";

        // ═══════════════════════════════════════════════════════
        // Call Analytics (🆕 Server-Side)
        // ═══════════════════════════════════════════════════════
        public const string AnalyticsSummary = "/api/CallAnalytics/Summary";
        public const string AnalyticsTopContacts = "/api/CallAnalytics/TopContacts/{0}";
        public const string AnalyticsDistribution = "/api/CallAnalytics/Distribution/{0}";
        public const string AnalyticsTeamComparison = "/api/CallAnalytics/TeamComparison/{0}";
        public const string AnalyticsContactDetail = "/api/CallAnalytics/ContactDetail";

        // ═══════════════════════════════════════════════════════
        // Device Management (🆕)
        // ═══════════════════════════════════════════════════════
        public const string DeviceList = "/api/DeviceManagement/List/{0}";
        public const string DeviceStatus = "/api/DeviceManagement/{0}/Status";
        public const string DeviceRegister = "/api/DeviceManagement/Register";

        // ═══════════════════════════════════════════════════════
        // Sync Management (🆕)
        // ═══════════════════════════════════════════════════════
        public const string SyncManagementHistory = "/api/SyncManagement/History/{0}";
        public const string SyncManagementStatus = "/api/SyncManagement/Status/{0}";

        // ═══════════════════════════════════════════════════════
        // Export (🆕)
        // ═══════════════════════════════════════════════════════
        public const string ExportCallLogs = "/api/Export/CallLogs";
        public const string ExportAnalyticsReport = "/api/Export/AnalyticsReport";

        // ═══════════════════════════════════════════════════════
        // Settings / Employee / Company
        // ═══════════════════════════════════════════════════════
        public const string Settings = "/api/settings";
        public const string GetEmployee = "/api/EmployeeMaster/{0}";
        public const string GetCompany = "/api/CompanyMaster/{0}";
    }
}
