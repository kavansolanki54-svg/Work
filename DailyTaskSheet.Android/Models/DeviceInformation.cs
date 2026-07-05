using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Stores device hardware and software information.
    /// Used for API sync payloads and local diagnostics.
    /// </summary>
    [Table("DeviceInformation")]
    public class DeviceInformation
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Unique device identifier (ANDROID_ID).</summary>
        [MaxLength(100), Indexed(Unique = true)]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Device manufacturer (e.g., "Samsung", "Google").</summary>
        [MaxLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>Device model name (e.g., "SM-S911B", "Pixel 8").</summary>
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        /// <summary>Device brand (e.g., "samsung", "google").</summary>
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>Android version string (e.g., "14").</summary>
        [MaxLength(20)]
        public string AndroidVersion { get; set; } = string.Empty;

        /// <summary>Android API level (e.g., 34).</summary>
        public int ApiLevel { get; set; }

        /// <summary>Android build display (e.g., "UP1A.231005.007").</summary>
        [MaxLength(100)]
        public string BuildDisplay { get; set; } = string.Empty;

        /// <summary>Device product name.</summary>
        [MaxLength(100)]
        public string Product { get; set; } = string.Empty;

        /// <summary>Device hardware name.</summary>
        [MaxLength(100)]
        public string Hardware { get; set; } = string.Empty;

        /// <summary>Screen density DPI.</summary>
        public int ScreenDensityDpi { get; set; }

        /// <summary>Screen width in pixels.</summary>
        public int ScreenWidthPixels { get; set; }

        /// <summary>Screen height in pixels.</summary>
        public int ScreenHeightPixels { get; set; }

        /// <summary>Total RAM in megabytes.</summary>
        public long TotalRamMb { get; set; }

        /// <summary>Timestamp when this record was last updated.</summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
