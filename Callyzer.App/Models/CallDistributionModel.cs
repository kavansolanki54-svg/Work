using System.Collections.Generic;

namespace Callyzer.App.Models
{
    /// <summary>
    /// Represents a distribution of call data across buckets (hours, durations, days of week, SIM slots).
    /// Used to render bar charts, heatmaps, and distribution visualizations.
    /// </summary>
    public class CallDistributionModel
    {
        /// <summary>Type of distribution: "Hourly", "Duration", "DayOfWeek", "Sim".</summary>
        public string DistributionType { get; set; } = string.Empty;

        /// <summary>The individual buckets of the distribution.</summary>
        public List<DistributionBucket> Buckets { get; set; } = new();
    }

    /// <summary>
    /// Represents a single bucket in a call distribution.
    /// </summary>
    public class DistributionBucket
    {
        /// <summary>Human-readable label (e.g., "9 AM", "1-5m", "Monday", "SIM 1").</summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>Number of calls in this bucket.</summary>
        public int Count { get; set; }

        /// <summary>Total duration of calls in this bucket (seconds).</summary>
        public long TotalDuration { get; set; }

        /// <summary>Percentage of total calls represented by this bucket.</summary>
        public double Percentage { get; set; }
    }
}
