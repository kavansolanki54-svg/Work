using System;

namespace Callyzer.App.Models
{
    /// <summary>
    /// Represents aggregated call analytics for a specific contact (phone number).
    /// Used for per-contact drill-down and contact ranking views.
    /// </summary>
    public class ContactAnalyticsModel
    {
        /// <summary>The phone number (normalized).</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>The contact name from the device contacts database.</summary>
        public string ContactName { get; set; } = string.Empty;

        /// <summary>URI to the contact's photo (platform-specific).</summary>
        public string? ContactPhotoUri { get; set; }

        // ─── Call Counts ───────────────────────────────────────

        /// <summary>Total calls with this contact.</summary>
        public int TotalCalls { get; set; }

        /// <summary>Incoming calls from this contact.</summary>
        public int IncomingCalls { get; set; }

        /// <summary>Outgoing calls to this contact.</summary>
        public int OutgoingCalls { get; set; }

        /// <summary>Missed calls from this contact.</summary>
        public int MissedCalls { get; set; }

        // ─── Duration Metrics ──────────────────────────────────

        /// <summary>Total duration of all calls (seconds).</summary>
        public long TotalDuration { get; set; }

        /// <summary>Average call duration (seconds).</summary>
        public double AverageDuration { get; set; }

        /// <summary>Longest single call duration (seconds).</summary>
        public int LongestCallDuration { get; set; }

        // ─── Timeline ──────────────────────────────────────────

        /// <summary>Date of the first call with this contact.</summary>
        public DateTime FirstCallDate { get; set; }

        /// <summary>Date of the most recent call with this contact.</summary>
        public DateTime LastCallDate { get; set; }

        // ─── Ranking ───────────────────────────────────────────

        /// <summary>Rank position in the top callers list (1-based).</summary>
        public int Rank { get; set; }
    }
}
