using System;

namespace Callyzer.App.Helpers
{
    /// <summary>
    /// Provides date/time conversion utilities for Unix timestamps and timezone handling.
    /// Android call log dates are stored as Unix epoch milliseconds.
    /// </summary>
    public static class DateTimeHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts Unix epoch milliseconds to a UTC DateTime.
        /// </summary>
        public static DateTime FromUnixMilliseconds(long milliseconds)
        {
            if (milliseconds <= 0) return DateTime.MinValue;
            return UnixEpoch.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Converts a DateTime to Unix epoch milliseconds.
        /// </summary>
        public static long ToUnixMilliseconds(DateTime dateTime)
        {
            if (dateTime <= UnixEpoch) return 0;
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Converts Unix epoch seconds to a UTC DateTime.
        /// </summary>
        public static DateTime FromUnixSeconds(long seconds)
        {
            if (seconds <= 0) return DateTime.MinValue;
            return UnixEpoch.AddSeconds(seconds);
        }

        /// <summary>
        /// Converts a DateTime to Unix epoch seconds.
        /// </summary>
        public static long ToUnixSeconds(DateTime dateTime)
        {
            if (dateTime <= UnixEpoch) return 0;
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Gets the start of today in UTC.
        /// </summary>
        public static DateTime TodayStartUtc => DateTime.UtcNow.Date;

        /// <summary>
        /// Calculates the call end time from start time and duration.
        /// </summary>
        public static DateTime CalculateEndTime(DateTime startTime, int durationSeconds)
        {
            if (durationSeconds <= 0) return startTime;
            return startTime.AddSeconds(durationSeconds);
        }

        /// <summary>
        /// Gets the current timezone ID string.
        /// </summary>
        public static string GetCurrentTimeZoneId()
        {
            return TimeZoneInfo.Local.Id;
        }

        /// <summary>
        /// Gets the start of the week (Monday) for a given date.
        /// </summary>
        public static DateTime GetWeekStart(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.Date.AddDays(-diff);
        }

        /// <summary>
        /// Gets the start of the month for a given date.
        /// </summary>
        public static DateTime GetMonthStart(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets the end of the month for a given date.
        /// </summary>
        public static DateTime GetMonthEnd(DateTime date)
        {
            return GetMonthStart(date).AddMonths(1);
        }
    }
}
