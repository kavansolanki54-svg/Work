using System;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Extensions;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Managers;

namespace DailyTaskSheet.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Dashboard screen.
    /// Provides real-time statistics, sync status, and action commands.
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly ICallLogRepository _callLogRepo;
        private readonly ISyncRepository _syncRepo;
        private readonly ISyncService _syncService;
        private readonly INetworkService _networkService;
        private readonly IDeviceService _deviceService;
        private readonly IPreferenceService _preferenceService;
        private readonly ILoggerService _logger;

        private string _employeeName = string.Empty;
        private int _employeeId;
        private string _companyName = string.Empty;
        private string _deviceName = string.Empty;
        private string _androidVersion = string.Empty;
        private string _lastSyncTime = "Never";
        private string _nextSyncTime = "Not scheduled";
        private int _todayCallCount;
        private int _pendingCount;
        private int _failedCount;
        private string _lastApiStatus = "N/A";
        private string _internetStatus = "Checking...";
        private string _serviceStatus = "Stopped";
        private bool _isSyncing;

        // Properties
        public string EmployeeName { get => _employeeName; set => SetProperty(ref _employeeName, value); }
        public int EmployeeId { get => _employeeId; set => SetProperty(ref _employeeId, value); }
        public string CompanyName { get => _companyName; set => SetProperty(ref _companyName, value); }
        public string DeviceName { get => _deviceName; set => SetProperty(ref _deviceName, value); }
        public string AndroidVersion { get => _androidVersion; set => SetProperty(ref _androidVersion, value); }
        public string LastSyncTime { get => _lastSyncTime; set => SetProperty(ref _lastSyncTime, value); }
        public string NextSyncTime { get => _nextSyncTime; set => SetProperty(ref _nextSyncTime, value); }
        public int TodayCallCount { get => _todayCallCount; set => SetProperty(ref _todayCallCount, value); }
        public int PendingCount { get => _pendingCount; set => SetProperty(ref _pendingCount, value); }
        public int FailedCount { get => _failedCount; set => SetProperty(ref _failedCount, value); }
        public string LastApiStatus { get => _lastApiStatus; set => SetProperty(ref _lastApiStatus, value); }
        public string InternetStatus { get => _internetStatus; set => SetProperty(ref _internetStatus, value); }
        public string ServiceStatus { get => _serviceStatus; set => SetProperty(ref _serviceStatus, value); }
        public bool IsSyncingNow { get => _isSyncing; set => SetProperty(ref _isSyncing, value); }

        /// <summary>
        /// Initializes a new instance of <see cref="DashboardViewModel"/>.
        /// </summary>
        public DashboardViewModel(
            IAuthenticationService authService,
            ICallLogRepository callLogRepo,
            ISyncRepository syncRepo,
            ISyncService syncService,
            INetworkService networkService,
            IDeviceService deviceService,
            IPreferenceService preferenceService,
            ILoggerService logger)
        {
            _authService = authService;
            _callLogRepo = callLogRepo;
            _syncRepo = syncRepo;
            _syncService = syncService;
            _networkService = networkService;
            _deviceService = deviceService;
            _preferenceService = preferenceService;
            _logger = logger;

            Title = "Dashboard";
        }

        /// <summary>
        /// Loads all dashboard data from local storage and services.
        /// </summary>
        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                // User info
                EmployeeName = _authService.GetUserName();
                EmployeeId = _authService.GetEmployeeId();
                CompanyName = $"Company #{_authService.GetCompanyId()}";

                // Device info
                DeviceName = _deviceService.GetDeviceName();
                AndroidVersion = $"Android {Android.OS.Build.VERSION.Release}";

                // Sync stats
                TodayCallCount = await _callLogRepo.GetTodayCallCountAsync();
                PendingCount = await _callLogRepo.GetPendingCountAsync();
                FailedCount = await _callLogRepo.GetFailedCountAsync();

                // Last sync time
                string lastSync = _preferenceService.GetString(AppConstants.PrefKeyLastSyncTime);
                if (!string.IsNullOrEmpty(lastSync) && DateTime.TryParse(lastSync, out DateTime lastSyncDt))
                {
                    LastSyncTime = lastSyncDt.ToRelativeTime();
                }
                else
                {
                    LastSyncTime = "Never";
                }

                // Next sync time
                int interval = _preferenceService.GetInt(AppConstants.PrefKeySyncInterval, AppConstants.DefaultSyncIntervalMinutes);
                if (!string.IsNullOrEmpty(lastSync) && DateTime.TryParse(lastSync, out DateTime lastDt))
                {
                    NextSyncTime = lastDt.AddMinutes(interval).ToFutureRelativeTime();
                }

                // Last API status
                var lastSyncHistory = await _syncRepo.GetLastSyncAsync();
                if (lastSyncHistory != null)
                {
                    LastApiStatus = lastSyncHistory.Status == 2 ? "✓ Success" : "✗ Failed";
                }

                // Network
                InternetStatus = _networkService.CurrentStatus.ToString();

                // Service
                ServiceStatus = _preferenceService.GetBool(AppConstants.PrefKeyBackgroundSyncEnabled, true)
                    ? "Running" : "Stopped";

                IsSyncingNow = _syncService.IsSyncing;
            }
            catch (Exception ex)
            {
                _logger.Error("DashboardVM", "Failed to load dashboard data", ex);
                ErrorMessage = "Failed to load data.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Triggers an immediate manual synchronization.
        /// </summary>
        public async Task SyncNowAsync()
        {
            if (IsSyncingNow) return;

            IsSyncingNow = true;
            try
            {
                var result = await _syncService.ExecuteSyncAsync("Manual", CancellationToken.None);
                _logger.Info("DashboardVM", $"Manual sync result: {result.Message}");
                await LoadDataAsync(); // Refresh stats
            }
            catch (Exception ex)
            {
                _logger.Error("DashboardVM", "Manual sync failed", ex);
                ErrorMessage = $"Sync failed: {ex.Message}";
            }
            finally
            {
                IsSyncingNow = false;
            }
        }
    }
}
