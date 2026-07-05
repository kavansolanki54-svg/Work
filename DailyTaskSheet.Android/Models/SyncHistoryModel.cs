using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Records the outcome of each synchronization attempt.
    /// Provides historical tracking and diagnostics.
    /// </summary>
    [Table("SyncHistory")]
    public class SyncHistoryModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Timestamp when the sync was initiated.</summary>
        [Indexed]
        public DateTime SyncTime { get; set; } = DateTime.UtcNow;

        /// <summary>Total call log records included in this sync batch.</summary>
        public int TotalCalls { get; set; }

        /// <summary>Number of records successfully uploaded.</summary>
        public int Uploaded { get; set; }

        /// <summary>Number of duplicate records detected by the server.</summary>
        public int Duplicates { get; set; }

        /// <summary>Number of records that failed to upload.</summary>
        public int Failed { get; set; }

        /// <summary>The sync status (0=Pending, 1=InProgress, 2=Success, 3=Failed).</summary>
        public int Status { get; set; }

        /// <summary>Human-readable result message from the API or local operation.</summary>
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        /// <summary>The source that triggered this sync (WorkManager, Manual, Boot, etc.).</summary>
        [MaxLength(50)]
        public string TriggerSource { get; set; } = string.Empty;

        /// <summary>Duration of the sync operation in milliseconds.</summary>
        public long DurationMs { get; set; }
    }
}
