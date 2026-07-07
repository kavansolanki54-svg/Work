using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Enums;
using Callyzer.App.Helpers;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Newtonsoft.Json;

namespace Callyzer.App.Services
{
    /// <summary>
    /// Core analytics engine — computes all analytics from local SQLite data.
    /// Implements caching for repeated queries and trend calculation.
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _repo;
        private readonly ILoggerService _logger;
        private const string Tag = "AnalyticsService";

        public AnalyticsService(IAnalyticsRepository repo, ILoggerService logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<AnalyticsSummaryModel> GetSummaryAsync(
            TimePeriodEnum period, DateTime? referenceDate = null, CancellationToken ct = default)
        {
            var refDate = referenceDate ?? DateTime.UtcNow;
            var (from, to, label) = GetPeriodRange(period, refDate);
            var (prevFrom, prevTo) = GetPreviousPeriodRange(period, refDate);

            // Check cache
            var periodStr = period.ToString();
            var cached = await _repo.GetCachedSummaryAsync(periodStr, from);
            if (cached != null && !string.IsNullOrEmpty(cached.JsonData))
            {
                try
                {
                    var cachedSummary = JsonConvert.DeserializeObject<AnalyticsSummaryModel>(cached.JsonData);
                    if (cachedSummary != null)
                    {
                        _logger.Debug(Tag, $"Returning cached summary for {period}: {from:d}");
                        return cachedSummary;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(Tag, $"Failed to deserialize cached summary: {ex.Message}");
                }
            }

            _logger.Debug(Tag, $"Computing summary for {period}: {from:d} to {to:d}");

            var summary = await _repo.GetSummaryForRangeAsync(from, to, ct);
            summary.PeriodLabel = label;

            // Compute trends
            var (curCount, curDuration, prevCount, prevDuration) =
                await _repo.GetTrendDataAsync(from, to, prevFrom, prevTo, ct);

            summary.CallCountTrendPercent = prevCount > 0
                ? ((curCount - prevCount) * 100.0 / prevCount) : 0;
            summary.DurationTrendPercent = prevDuration > 0
                ? ((curDuration - prevDuration) * 100.0 / prevDuration) : 0;

            // Save to cache
            var cacheModel = new AnalyticsCacheModel
            {
                PeriodType = periodStr,
                PeriodStart = from,
                PeriodEnd = to,
                JsonData = JsonConvert.SerializeObject(summary)
            };
            await _repo.SaveCachedSummaryAsync(cacheModel);

            _logger.Info(Tag, $"Summary computed and cached: {summary.TotalCalls} calls");
            return summary;
        }

        public async Task<AnalyticsSummaryModel> GetCustomRangeSummaryAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var summary = await _repo.GetSummaryForRangeAsync(from, to, ct);
            summary.PeriodLabel = $"{from:MMM dd} – {to:MMM dd, yyyy}";

            // Compute trends against same-length previous period
            var length = to - from;
            var prevFrom = from - length;
            var prevTo = from;
            var (curCount, curDuration, prevCount, prevDuration) =
                await _repo.GetTrendDataAsync(from, to, prevFrom, prevTo, ct);

            summary.CallCountTrendPercent = prevCount > 0
                ? ((curCount - prevCount) * 100.0 / prevCount) : 0;
            summary.DurationTrendPercent = prevDuration > 0
                ? ((curDuration - prevDuration) * 100.0 / prevDuration) : 0;

            return summary;
        }

        public async Task<List<ContactAnalyticsModel>> GetTopContactsAsync(
            int limit, DateTime from, DateTime to,
            SortOrderEnum sortBy = SortOrderEnum.ByCount, CancellationToken ct = default)
        {
            return sortBy switch
            {
                SortOrderEnum.ByDuration => await _repo.GetTopContactsByDurationAsync(limit, 0, from, to, ct),
                _ => await _repo.GetTopContactsByCountAsync(limit, 0, from, to, ct)
            };
        }

        public async Task<List<CallLogModel>> GetLongestCallsAsync(
            int limit, DateTime from, DateTime to, CancellationToken ct = default)
        {
            // This could use a raw SQL query for better performance, but
            // for simplicity we use the repository pattern
            _logger.Debug(Tag, $"Loading {limit} longest calls from {from:d} to {to:d}");
            return new List<CallLogModel>(); // TODO: implement raw SQL ORDER BY Duration DESC LIMIT
        }

        public async Task<ContactAnalyticsModel> GetContactAnalyticsAsync(
            string phoneNumber, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var result = await _repo.GetContactSummaryAsync(phoneNumber, from, to, ct);
            return result ?? new ContactAnalyticsModel
            {
                PhoneNumber = phoneNumber,
                ContactName = phoneNumber
            };
        }

        public async Task<ComparisonResultModel> CompareContactsAsync(
            string[] phoneNumbers, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var contacts = new List<ContactAnalyticsModel>();
            foreach (var number in phoneNumbers)
            {
                ct.ThrowIfCancellationRequested();
                var analytics = await _repo.GetContactSummaryAsync(number, from, to, ct);
                if (analytics != null) contacts.Add(analytics);
            }

            return new ComparisonResultModel
            {
                From = from,
                To = to,
                Contacts = contacts
            };
        }

        public async Task<CallDistributionModel> GetHourlyDistributionAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var buckets = await _repo.GetHourlyDistributionAsync(from, to, ct);
            return new CallDistributionModel { DistributionType = "Hourly", Buckets = buckets };
        }

        public async Task<CallDistributionModel> GetDurationBucketDistributionAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var buckets = await _repo.GetDurationBucketDistributionAsync(from, to, ct);
            return new CallDistributionModel { DistributionType = "Duration", Buckets = buckets };
        }

        public async Task<CallDistributionModel> GetDayOfWeekDistributionAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var buckets = await _repo.GetDayOfWeekDistributionAsync(from, to, ct);
            return new CallDistributionModel { DistributionType = "DayOfWeek", Buckets = buckets };
        }

        public async Task<CallDistributionModel> GetSimDistributionAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var buckets = await _repo.GetSimDistributionAsync(from, to, ct);
            return new CallDistributionModel { DistributionType = "Sim", Buckets = buckets };
        }

        public async Task InvalidateCacheAsync(TimePeriodEnum? period = null)
        {
            await _repo.InvalidateCacheAsync();
            _logger.Info(Tag, $"Cache invalidated for: {period?.ToString() ?? "ALL"}");
        }

        // ─── Period Range Helpers ────────────────────────────────

        private static (DateTime from, DateTime to, string label) GetPeriodRange(
            TimePeriodEnum period, DateTime refDate)
        {
            return period switch
            {
                TimePeriodEnum.Daily => (
                    refDate.Date,
                    refDate.Date.AddDays(1),
                    refDate.Date == DateTime.UtcNow.Date ? "Today" : refDate.ToString("MMM dd, yyyy")),

                TimePeriodEnum.Weekly => (
                    DateTimeHelper.GetWeekStart(refDate),
                    DateTimeHelper.GetWeekStart(refDate).AddDays(7),
                    "This Week"),

                TimePeriodEnum.Monthly => (
                    DateTimeHelper.GetMonthStart(refDate),
                    DateTimeHelper.GetMonthEnd(refDate),
                    refDate.ToString("MMMM yyyy")),

                _ => (refDate.Date, refDate.Date.AddDays(1), "Today")
            };
        }

        private static (DateTime prevFrom, DateTime prevTo) GetPreviousPeriodRange(
            TimePeriodEnum period, DateTime refDate)
        {
            return period switch
            {
                TimePeriodEnum.Daily => (refDate.Date.AddDays(-1), refDate.Date),
                TimePeriodEnum.Weekly => (DateTimeHelper.GetWeekStart(refDate).AddDays(-7), DateTimeHelper.GetWeekStart(refDate)),
                TimePeriodEnum.Monthly => (DateTimeHelper.GetMonthStart(refDate).AddMonths(-1), DateTimeHelper.GetMonthStart(refDate)),
                _ => (refDate.Date.AddDays(-1), refDate.Date)
            };
        }
    }
}
