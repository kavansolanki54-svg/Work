namespace Callyzer.App.Enums
{
    /// <summary>
    /// Represents the time period for analytics aggregation.
    /// </summary>
    public enum TimePeriodEnum
    {
        /// <summary>Analytics for the current day.</summary>
        Daily = 0,

        /// <summary>Analytics for the current week (Mon–Sun).</summary>
        Weekly = 1,

        /// <summary>Analytics for the current calendar month.</summary>
        Monthly = 2,

        /// <summary>Analytics for a user-defined date range.</summary>
        Custom = 3
    }
}
