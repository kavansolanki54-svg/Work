using System;

namespace DailyTaskSheet.App.Extensions
{
    /// <summary>
    /// Extension methods for DateTime formatting and relative time display.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Formats a DateTime as a relative time string (e.g., "2 hours ago", "Just now").
        /// </summary>
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var now = DateTime.UtcNow;
            var diff = now - dateTime.ToUniversalTime();

            if (diff.TotalSeconds < 60) return "Just now";
            if (diff.TotalMinutes < 2) return "1 minute ago";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} minutes ago";
            if (diff.TotalHours < 2) return "1 hour ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hours ago";
            if (diff.TotalDays < 2) return "Yesterday";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} days ago";
            if (diff.TotalDays < 14) return "1 week ago";
            if (diff.TotalDays < 30) return $"{(int)(diff.TotalDays / 7)} weeks ago";
            if (diff.TotalDays < 60) return "1 month ago";
            if (diff.TotalDays < 365) return $"{(int)(diff.TotalDays / 30)} months ago";

            return dateTime.ToString("MMM dd, yyyy");
        }

        /// <summary>
        /// Formats a DateTime for display in the app (short date + time).
        /// </summary>
        public static string ToDisplayString(this DateTime dateTime)
        {
            var local = dateTime.ToLocalTime();
            return local.ToString("MMM dd, yyyy  hh:mm tt");
        }

        /// <summary>
        /// Formats a DateTime as time only (e.g., "02:30 PM").
        /// </summary>
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("hh:mm tt");
        }

        /// <summary>
        /// Formats a DateTime as date only (e.g., "Jan 15, 2025").
        /// </summary>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("MMM dd, yyyy");
        }

        /// <summary>
        /// Formats a duration in seconds as a human-readable string (e.g., "2m 30s").
        /// </summary>
        public static string FormatDuration(int durationSeconds)
        {
            if (durationSeconds <= 0) return "0s";
            var span = TimeSpan.FromSeconds(durationSeconds);
            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours}h {span.Minutes}m {span.Seconds}s";
            if (span.TotalMinutes >= 1)
                return $"{span.Minutes}m {span.Seconds}s";
            return $"{span.Seconds}s";
        }

        /// <summary>
        /// Returns a future time formatted as "in X minutes/hours" for next sync display.
        /// </summary>
        public static string ToFutureRelativeTime(this DateTime futureTime)
        {
            var diff = futureTime.ToUniversalTime() - DateTime.UtcNow;
            if (diff.TotalSeconds <= 0) return "Now";
            if (diff.TotalMinutes < 2) return "in 1 minute";
            if (diff.TotalMinutes < 60) return $"in {(int)diff.TotalMinutes} minutes";
            if (diff.TotalHours < 2) return "in 1 hour";
            return $"in {(int)diff.TotalHours} hours";
        }
    }
}
