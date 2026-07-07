using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Enums;
using Callyzer.App.Extensions;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Dashboard page — the main analytics hub.
    /// Displays summary stats, quick charts, and recent call activity.
    /// </summary>
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ICallLogPlatformService _callLogPlatform;
        private readonly ILoggerService _logger;

        [ObservableProperty]
        private AnalyticsSummaryModel? _todaySummary;

        [ObservableProperty]
        private AnalyticsSummaryModel? _weekSummary;

        [ObservableProperty]
        private int _totalLocalCallLogs;

        [ObservableProperty]
        private int _pendingSyncCount;

        [ObservableProperty]
        private string _lastSyncTimeDisplay = "Never";

        [ObservableProperty]
        private ObservableCollection<ContactAnalyticsModel> _topContacts = new();

        [ObservableProperty]
        private bool _isCallLogAccessSupported;

        [ObservableProperty]
        private bool _hasPermissions;

        public DashboardViewModel(
            IAnalyticsService analyticsService,
            ICallLogPlatformService callLogPlatform,
            ILoggerService logger)
        {
            _analyticsService = analyticsService;
            _callLogPlatform = callLogPlatform;
            _logger = logger;
            Title = "Dashboard";
            IsCallLogAccessSupported = callLogPlatform.IsCallLogAccessSupported;

            WeakReferenceMessenger.Default.Register<SyncCompletedMessage>(this, async (r, m) =>
            {
                if (m.Success)
                {
                    _logger.Info("DashboardVM", $"Sync completed ({m.SyncedCount} items). Refreshing dashboard...");
                    await LoadDashboardAsync();
                }
            });
        }

        [RelayCommand]
        private async Task LoadDashboardAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                _logger.Info("DashboardVM", "Loading dashboard data...");

                HasPermissions = _callLogPlatform.HasRequiredPermissions;

                // Load today's summary
                TodaySummary = await _analyticsService.GetSummaryAsync(TimePeriodEnum.Daily);

                // Load this week's summary
                WeekSummary = await _analyticsService.GetSummaryAsync(TimePeriodEnum.Weekly);

                // Load top 5 contacts for this week
                var now = DateTime.UtcNow;
                var weekStart = Helpers.DateTimeHelper.GetWeekStart(now);
                var topList = await _analyticsService.GetTopContactsAsync(
                    5, weekStart, now, SortOrderEnum.ByCount);
                TopContacts = new ObservableCollection<ContactAnalyticsModel>(topList);

                _logger.Info("DashboardVM", $"Dashboard loaded: {TodaySummary?.TotalCalls ?? 0} calls today");
            }, "Failed to load dashboard");
        }

        [RelayCommand]
        private async Task RequestPermissionsAsync()
        {
            var granted = await _callLogPlatform.RequestPermissionsAsync();
            HasPermissions = granted;
            if (granted)
            {
                await LoadDashboardAsync();
            }
        }

        [RelayCommand]
        private async Task NavigateToAnalyticsAsync()
        {
            await Shell.Current.GoToAsync("//analytics");
        }

        [RelayCommand]
        private async Task NavigateToContactDetailAsync(ContactAnalyticsModel contact)
        {
            if (contact == null) return;
            var encodedNumber = Uri.EscapeDataString(contact.PhoneNumber);
            await Shell.Current.GoToAsync($"contactdetail?phoneNumber={encodedNumber}");
        }
    }
}
