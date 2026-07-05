using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Records individual API call details for diagnostics and audit.
    /// Every HTTP request/response pair is logged here.
    /// </summary>
    [Table("ApiLogs")]
    public class ApiLogModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>The API endpoint that was called.</summary>
        [MaxLength(300)]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>HTTP method (GET, POST, PUT, DELETE).</summary>
        [MaxLength(10)]
        public string Method { get; set; } = string.Empty;

        /// <summary>HTTP status code returned by the server.</summary>
        public int StatusCode { get; set; }

        /// <summary>Truncated request body (first 2000 chars, sensitive data masked).</summary>
        [MaxLength(2000)]
        public string RequestBody { get; set; } = string.Empty;

        /// <summary>Truncated response body (first 2000 chars).</summary>
        [MaxLength(2000)]
        public string ResponseBody { get; set; } = string.Empty;

        /// <summary>Request duration in milliseconds.</summary>
        public long DurationMs { get; set; }

        /// <summary>Whether the request was successful (2xx status).</summary>
        public bool IsSuccess { get; set; }

        /// <summary>Error message if the request failed.</summary>
        [MaxLength(500)]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>Timestamp of the API call.</summary>
        [Indexed]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
