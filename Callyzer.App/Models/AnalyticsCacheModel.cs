using System;
using SQLite;

namespace Callyzer.App.Models
{
    /// <summary>
    /// Caches computed analytics summaries in SQLite to avoid re-computation.
    /// Invalidated when new call logs are captured or synced.
    /// </summary>
    [Table("AnalyticsCache")]
    public class AnalyticsCacheModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Period type: "Daily", "Weekly", "Monthly".</summary>
        [MaxLength(20)]
        public string PeriodType { get; set; } = string.Empty;

        /// <summary>Start of the cached period.</summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>End of the cached period.</summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>Serialized JSON of AnalyticsSummaryModel.</summary>
        [MaxLength(8000)]
        public string JsonData { get; set; } = string.Empty;

        /// <summary>When this cache entry was created.</summary>
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Whether this cache entry has been invalidated.</summary>
        public bool IsStale { get; set; } = false;
    }
}
