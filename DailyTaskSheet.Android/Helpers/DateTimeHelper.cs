using System;

namespace DailyTaskSheet.App.Helpers
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
        /// <param name="milliseconds">Milliseconds since Unix epoch.</param>
        /// <returns>The corresponding UTC DateTime.</returns>
        public static DateTime FromUnixMilliseconds(long milliseconds)
        {
            if (milliseconds <= 0) return DateTime.MinValue;
            return UnixEpoch.AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// Converts a DateTime to Unix epoch milliseconds.
        /// </summary>
        /// <param name="dateTime">The DateTime to convert.</param>
        /// <returns>Milliseconds since Unix epoch.</returns>
        public static long ToUnixMilliseconds(DateTime dateTime)
        {
            if (dateTime <= UnixEpoch) return 0;
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Converts Unix epoch seconds to a UTC DateTime.
        /// </summary>
        /// <param name="seconds">Seconds since Unix epoch.</param>
        /// <returns>The corresponding UTC DateTime.</returns>
        public static DateTime FromUnixSeconds(long seconds)
        {
            if (seconds <= 0) return DateTime.MinValue;
            return UnixEpoch.AddSeconds(seconds);
        }

        /// <summary>
        /// Converts a DateTime to Unix epoch seconds.
        /// </summary>
        /// <param name="dateTime">The DateTime to convert.</param>
        /// <returns>Seconds since Unix epoch.</returns>
        public static long ToUnixSeconds(DateTime dateTime)
        {
            if (dateTime <= UnixEpoch) return 0;
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Gets the start of today in the device's local timezone, converted to UTC.
        /// </summary>
        public static DateTime TodayStartUtc => DateTime.UtcNow.Date;

        /// <summary>
        /// Calculates the call end time from start time and duration.
        /// </summary>
        /// <param name="startTime">The call start time.</param>
        /// <param name="durationSeconds">The call duration in seconds.</param>
        /// <returns>The calculated end time.</returns>
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
    }
}
