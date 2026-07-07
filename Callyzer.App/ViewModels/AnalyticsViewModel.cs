using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Enums;
using Callyzer.App.Extensions;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Microcharts;
using SkiaSharp;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// ViewModel for the full Analytics page with period tabs, charts, and rankings.
    /// </summary>
    public partial class AnalyticsViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILoggerService _logger;

        [ObservableProperty]
        private AnalyticsSummaryModel? _summary;

        [ObservableProperty]
        private TimePeriodEnum _selectedPeriod = TimePeriodEnum.Daily;

        [ObservableProperty]
        private DateTime _customFrom = DateTime.UtcNow.Date.AddDays(-7);

        [ObservableProperty]
        private DateTime _customTo = DateTime.UtcNow.Date.AddDays(1);

        [ObservableProperty]
        private ObservableCollection<ContactAnalyticsModel> _topContacts = new();

        [ObservableProperty]
        private CallDistributionModel? _hourlyDistribution;

        [ObservableProperty]
        private CallDistributionModel? _durationDistribution;

        // ─── Chart Objects ─────────────────────────────────────
        [ObservableProperty]
        private Chart? _callTypeChart;

        [ObservableProperty]
        private Chart? _hourlyChart;

        [ObservableProperty]
        private Chart? _durationChart;

        public AnalyticsViewModel(IAnalyticsService analyticsService, ILoggerService logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
            Title = "Analytics";
        }

        [RelayCommand]
        private async Task LoadAnalyticsAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                _logger.Info("AnalyticsVM", $"Loading analytics for period: {SelectedPeriod}");

                // Load summary
                if (SelectedPeriod == TimePeriodEnum.Custom)
                {
                    Summary = await _analyticsService.GetCustomRangeSummaryAsync(CustomFrom, CustomTo);
                }
                else
                {
                    Summary = await _analyticsService.GetSummaryAsync(SelectedPeriod);
                }

                // Determine date range
                var (from, to) = GetDateRange();

                // Load top contacts
                var contacts = await _analyticsService.GetTopContactsAsync(10, from, to, SortOrderEnum.ByCount);
                TopContacts = new ObservableCollection<ContactAnalyticsModel>(contacts);

                // Load distributions
                HourlyDistribution = await _analyticsService.GetHourlyDistributionAsync(from, to);
                DurationDistribution = await _analyticsService.GetDurationBucketDistributionAsync(from, to);

                // Build charts
                BuildCallTypeChart();
                BuildHourlyChart();
                BuildDurationChart();

                _logger.Info("AnalyticsVM", $"Analytics loaded: {Summary?.TotalCalls} total calls");
            }, "Failed to load analytics");
        }

        [RelayCommand]
        private async Task ChangePeriodAsync(string period)
        {
            if (Enum.TryParse<TimePeriodEnum>(period, out var parsed))
            {
                SelectedPeriod = parsed;
                await LoadAnalyticsAsync();
            }
        }

        private (DateTime from, DateTime to) GetDateRange()
        {
            if (Summary != null)
                return (Summary.PeriodStart, Summary.PeriodEnd);

            var now = DateTime.UtcNow;
            return SelectedPeriod switch
            {
                TimePeriodEnum.Daily => (now.Date, now.Date.AddDays(1)),
                TimePeriodEnum.Weekly => (Helpers.DateTimeHelper.GetWeekStart(now), now),
                TimePeriodEnum.Monthly => (Helpers.DateTimeHelper.GetMonthStart(now), now),
                TimePeriodEnum.Custom => (CustomFrom, CustomTo),
                _ => (now.Date, now)
            };
        }

        private void BuildCallTypeChart()
        {
            if (Summary == null) return;

            CallTypeChart = new DonutChart
            {
                Entries = new[]
                {
                    new ChartEntry(Summary.IncomingCalls)
                    {
                        Label = "Incoming",
                        Color = SKColor.Parse("#4CAF50"),
                        ValueLabel = Summary.IncomingCalls.ToString()
                    },
                    new ChartEntry(Summary.OutgoingCalls)
                    {
                        Label = "Outgoing",
                        Color = SKColor.Parse("#2196F3"),
                        ValueLabel = Summary.OutgoingCalls.ToString()
                    },
                    new ChartEntry(Summary.MissedCalls)
                    {
                        Label = "Missed",
                        Color = SKColor.Parse("#F44336"),
                        ValueLabel = Summary.MissedCalls.ToString()
                    },
                    new ChartEntry(Summary.RejectedCalls)
                    {
                        Label = "Rejected",
                        Color = SKColor.Parse("#FF9800"),
                        ValueLabel = Summary.RejectedCalls.ToString()
                    },
                },
                HoleRadius = 0.6f,
                LabelTextSize = 28,
                BackgroundColor = SKColors.Transparent
            };
        }

        private void BuildHourlyChart()
        {
            if (HourlyDistribution?.Buckets == null || HourlyDistribution.Buckets.Count == 0) return;

            HourlyChart = new BarChart
            {
                Entries = HourlyDistribution.Buckets.Select(b => new ChartEntry(b.Count)
                {
                    Label = b.Label,
                    Color = SKColor.Parse("#2196F3"),
                    ValueLabel = b.Count.ToString()
                }).ToArray(),
                LabelTextSize = 24,
                ValueLabelTextSize = 20,
                BackgroundColor = SKColors.Transparent
            };
        }

        private void BuildDurationChart()
        {
            if (DurationDistribution?.Buckets == null || DurationDistribution.Buckets.Count == 0) return;

            var colors = new[] { "#E3F2FD", "#90CAF9", "#42A5F5", "#1E88E5", "#1565C0", "#0D47A1", "#01579B" };

            DurationChart = new BarChart
            {
                Entries = DurationDistribution.Buckets.Select((b, i) => new ChartEntry(b.Count)
                {
                    Label = b.Label,
                    Color = SKColor.Parse(colors[i % colors.Length]),
                    ValueLabel = b.Count.ToString()
                }).ToArray(),
                LabelTextSize = 22,
                ValueLabelTextSize = 20,
                BackgroundColor = SKColors.Transparent
            };
        }
    }
}
