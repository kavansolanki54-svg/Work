using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Records application errors and exceptions for local diagnostics.
    /// Captured by the LoggerService.
    /// </summary>
    [Table("ErrorLogs")]
    public class ErrorLogModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Source class or component that generated the error.</summary>
        [MaxLength(200)]
        public string Source { get; set; } = string.Empty;

        /// <summary>Error message text.</summary>
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>Full stack trace of the exception.</summary>
        [MaxLength(4000)]
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>Log level (Debug, Info, Warning, Error, Critical).</summary>
        [MaxLength(20)]
        public string Level { get; set; } = "Error";

        /// <summary>Additional contextual data as JSON string.</summary>
        [MaxLength(2000)]
        public string AdditionalData { get; set; } = string.Empty;

        /// <summary>Timestamp when the error occurred.</summary>
        [Indexed]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
