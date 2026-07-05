using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Records API requests that failed and need to be retried.
    /// Used for offline resilience — failed requests are stored and retried later.
    /// </summary>
    [Table("FailedRequests")]
    public class FailedRequestModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>The API endpoint that failed.</summary>
        [MaxLength(300)]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>HTTP method (GET, POST, PUT, DELETE).</summary>
        [MaxLength(10)]
        public string Method { get; set; } = string.Empty;

        /// <summary>The full request body that needs to be resent.</summary>
        public string RequestBody { get; set; } = string.Empty;

        /// <summary>Error message from the last attempt.</summary>
        [MaxLength(500)]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>HTTP status code from the last attempt (0 if no response).</summary>
        public int LastStatusCode { get; set; }

        /// <summary>Number of retry attempts made.</summary>
        public int RetryCount { get; set; }

        /// <summary>Maximum retries before abandoning.</summary>
        public int MaxRetries { get; set; } = 10;

        /// <summary>Timestamp when this record was first created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Timestamp of the last retry attempt.</summary>
        public DateTime? LastAttemptAt { get; set; }

        /// <summary>Scheduled time for the next retry.</summary>
        public DateTime? NextRetryAt { get; set; }

        /// <summary>Whether this request has been permanently abandoned.</summary>
        public bool IsAbandoned { get; set; }
    }
}
