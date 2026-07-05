using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Tracks call log records that are pending synchronization.
    /// Manages retry logic and backoff scheduling.
    /// </summary>
    [Table("PendingSync")]
    public class PendingSyncModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Foreign key to the CallLogs table.</summary>
        [Indexed]
        public int CallLogId { get; set; }

        /// <summary>Number of retry attempts made for this record.</summary>
        public int RetryCount { get; set; }

        /// <summary>Maximum number of retries allowed.</summary>
        public int MaxRetries { get; set; } = 10;

        /// <summary>Timestamp of the last sync attempt.</summary>
        public DateTime? LastAttempt { get; set; }

        /// <summary>Scheduled time for the next retry attempt.</summary>
        public DateTime? NextAttempt { get; set; }

        /// <summary>Error message from the most recent failed attempt.</summary>
        [MaxLength(500)]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>Current status (0=Queued, 1=InProgress, 2=Completed, 3=Failed, 4=Abandoned).</summary>
        [Indexed]
        public int Status { get; set; }

        /// <summary>Timestamp when this pending record was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Timestamp when this record was last updated.</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
