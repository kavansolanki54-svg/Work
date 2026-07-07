namespace Callyzer.App.Enums
{
    /// <summary>
    /// Represents the sort order for analytics rankings.
    /// </summary>
    public enum SortOrderEnum
    {
        /// <summary>Sort by total call count (descending).</summary>
        ByCount = 0,

        /// <summary>Sort by total call duration (descending).</summary>
        ByDuration = 1,

        /// <summary>Sort by missed call count (descending).</summary>
        ByMissed = 2,

        /// <summary>Sort by most recent call date (descending).</summary>
        ByRecent = 3
    }
}
