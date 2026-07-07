using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Models;

namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Repository interface for analytics-specific SQL queries.
    /// Executes optimized raw SQL for high-performance aggregation over 50K+ records.
    /// </summary>
    public interface IAnalyticsRepository
    {
        Task<AnalyticsSummaryModel> GetSummaryForRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<List<ContactAnalyticsModel>> GetTopContactsByCountAsync(int limit, int offset, DateTime from, DateTime to, CancellationToken ct = default);
        Task<List<ContactAnalyticsModel>> GetTopContactsByDurationAsync(int limit, int offset, DateTime from, DateTime to, CancellationToken ct = default);
        Task<ContactAnalyticsModel?> GetContactSummaryAsync(string phoneNumber, DateTime from, DateTime to, CancellationToken ct = default);
        Task<List<DistributionBucket>> GetHourlyDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<List<DistributionBucket>> GetDurationBucketDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<List<DistributionBucket>> GetDayOfWeekDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<List<DistributionBucket>> GetSimDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<(int currentCount, long currentDuration, int previousCount, long previousDuration)> GetTrendDataAsync(DateTime currentFrom, DateTime currentTo, DateTime previousFrom, DateTime previousTo, CancellationToken ct = default);
        
        // Caching
        Task<AnalyticsCacheModel?> GetCachedSummaryAsync(string periodType, DateTime periodStart);
        Task SaveCachedSummaryAsync(AnalyticsCacheModel cacheModel);
        Task InvalidateCacheAsync();
    }
}
