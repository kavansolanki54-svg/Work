using System;

namespace Callyzer.App.Models
{
    /// <summary>
    /// Captures device hardware and software information for multi-device tracking.
    /// </summary>
    public class DeviceInformation
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string OsVersion { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty; // "Android" | "iOS"
        public int BatteryPercentage { get; set; }
        public string TimeZone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents metadata about a local database backup file.
    /// </summary>
    public class BackupInfoModel
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CallLogCount { get; set; }
        
        public string FormattedSize => FileSizeBytes < 1024 * 1024
            ? $"{FileSizeBytes / 1024.0:F1} KB"
            : $"{FileSizeBytes / (1024.0 * 1024.0):F2} MB";
            
        public string DisplayName => $"Backup_{CreatedAt:yyyy-MM-dd_HHmm}.zip";
    }

    /// <summary>
    /// Represents the configuration for an export operation.
    /// </summary>
    public class ExportConfigModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Enums.ExportFormatEnum Format { get; set; }
        public bool IncludeAnalytics { get; set; } = true;
        public bool IncludeCharts { get; set; } = false;
    }

    /// <summary>
    /// Wraps API response results.
    /// </summary>
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
