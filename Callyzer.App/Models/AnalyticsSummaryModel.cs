using System;
using System.Collections.Generic;

namespace Callyzer.App.Models
{
    /// <summary>
    /// Represents aggregated analytics for a time period (daily, weekly, monthly, or custom).
    /// Computed locally from SQLite data for instant offline access.
    /// </summary>
    public class AnalyticsSummaryModel
    {
        /// <summary>Start of the analytics period.</summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>End of the analytics period (exclusive).</summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>Human-readable period label (e.g., "Today", "This Week", "June 2026").</summary>
        public string PeriodLabel { get; set; } = string.Empty;

        // ─── Call Counts ───────────────────────────────────────

        /// <summary>Total number of calls in the period.</summary>
        public int TotalCalls { get; set; }

        /// <summary>Number of incoming calls answered.</summary>
        public int IncomingCalls { get; set; }

        /// <summary>Number of outgoing calls placed.</summary>
        public int OutgoingCalls { get; set; }

        /// <summary>Number of missed incoming calls.</summary>
        public int MissedCalls { get; set; }

        /// <summary>Number of calls rejected by the user.</summary>
        public int RejectedCalls { get; set; }

        // ─── Duration Metrics (seconds) ────────────────────────

        /// <summary>Total call duration across all calls (seconds).</summary>
        public long TotalDuration { get; set; }

        /// <summary>Total incoming call duration (seconds).</summary>
        public long IncomingDuration { get; set; }

        /// <summary>Total outgoing call duration (seconds).</summary>
        public long OutgoingDuration { get; set; }

        /// <summary>Average call duration across connected calls (seconds).</summary>
        public double AverageDuration { get; set; }

        /// <summary>Duration of the longest single call (seconds).</summary>
        public int LongestCallDuration { get; set; }

        // ─── Computed Metrics ──────────────────────────────────

        /// <summary>Percentage of total calls that were missed.</summary>
        public double MissedCallPercentage => TotalCalls > 0 ? (MissedCalls * 100.0 / TotalCalls) : 0;

        /// <summary>Percentage of incoming calls that were answered (not missed).</summary>
        public double AnswerRate => TotalCalls > 0 ? ((TotalCalls - MissedCalls) * 100.0 / TotalCalls) : 0;

        /// <summary>Number of unique phone numbers contacted.</summary>
        public int UniqueContacts { get; set; }

        // ─── Trends (compared to previous period) ──────────────

        /// <summary>Percentage change in call count vs. previous period. Positive = increase.</summary>
        public double CallCountTrendPercent { get; set; }

        /// <summary>Percentage change in total duration vs. previous period.</summary>
        public double DurationTrendPercent { get; set; }

        // ─── SIM Breakdown ─────────────────────────────────────

        /// <summary>Call count per SIM slot (key = SimSlot index, value = count).</summary>
        public Dictionary<int, int> CallsBySim { get; set; } = new();
    }
}
