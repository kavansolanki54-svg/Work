namespace DailyTaskSheet.App.Constants
{
    /// <summary>
    /// Centralizes all REST API endpoint paths.
    /// Ensures endpoint URLs are never hardcoded throughout the codebase.
    /// </summary>
    public static class ApiEndpoints
    {
        // ══════════════════════════════════════════════════════════════════
        // Authentication
        // ══════════════════════════════════════════════════════════════════

        /// <summary>POST - Authenticate user and receive JWT + refresh token.</summary>
        public const string Login = "/api/Auth/login";

        /// <summary>POST - Refresh an expired JWT using a valid refresh token.</summary>
        public const string RefreshToken = "/api/Auth/RefreshToken";

        // ══════════════════════════════════════════════════════════════════
        // Call Log Synchronization
        // ══════════════════════════════════════════════════════════════════

        /// <summary>POST - Upload a batch of call log records.</summary>
        public const string SyncCallLogs = "/api/CallLogs/Sync";

        /// <summary>POST - Upload a native audio recording file.</summary>
        public const string UploadNativeRecording = "/api/CallLogs/UploadNativeRecording";

        /// <summary>GET - Retrieve synchronization history records.</summary>
        public const string SyncHistory = "/api/CallLogs/History";

        // ══════════════════════════════════════════════════════════════════
        // Settings
        // ══════════════════════════════════════════════════════════════════

        /// <summary>GET - Retrieve server-side application settings.</summary>
        public const string Settings = "/api/settings";

        // ══════════════════════════════════════════════════════════════════
        // Employee / Company
        // ══════════════════════════════════════════════════════════════════

        /// <summary>GET - Retrieve employee details by ID.</summary>
        public const string GetEmployee = "/api/EmployeeMaster/{0}";

        /// <summary>GET - Retrieve company details by ID.</summary>
        public const string GetCompany = "/api/CompanyMaster/{0}";
    }
}
