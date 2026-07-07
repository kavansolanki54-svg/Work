using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Enums;
using Callyzer.App.Models;

namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Interface for the local analytics engine.
    /// Computes all analytics from local SQLite data for instant offline access.
    /// </summary>
    public interface IAnalyticsService
    {
        // ─── Summary Analytics ─────────────────────────────
        Task<AnalyticsSummaryModel> GetSummaryAsync(TimePeriodEnum period, DateTime? referenceDate = null, CancellationToken ct = default);
        Task<AnalyticsSummaryModel> GetCustomRangeSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default);

        // ─── Rankings ──────────────────────────────────────
        Task<List<ContactAnalyticsModel>> GetTopContactsAsync(int limit, DateTime from, DateTime to, SortOrderEnum sortBy = SortOrderEnum.ByCount, CancellationToken ct = default);
        Task<List<CallLogModel>> GetLongestCallsAsync(int limit, DateTime from, DateTime to, CancellationToken ct = default);

        // ─── Contact-Specific ──────────────────────────────
        Task<ContactAnalyticsModel> GetContactAnalyticsAsync(string phoneNumber, DateTime from, DateTime to, CancellationToken ct = default);
        Task<ComparisonResultModel> CompareContactsAsync(string[] phoneNumbers, DateTime from, DateTime to, CancellationToken ct = default);

        // ─── Distributions ─────────────────────────────────
        Task<CallDistributionModel> GetHourlyDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<CallDistributionModel> GetDurationBucketDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<CallDistributionModel> GetDayOfWeekDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<CallDistributionModel> GetSimDistributionAsync(DateTime from, DateTime to, CancellationToken ct = default);

        // ─── Cache Management ──────────────────────────────
        Task InvalidateCacheAsync(TimePeriodEnum? period = null);
    }
}
