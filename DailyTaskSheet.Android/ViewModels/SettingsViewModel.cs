using System;
using System.Threading.Tasks;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Managers;

namespace DailyTaskSheet.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Settings screen.
    /// Provides all configurable application settings with change persistence.
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IPreferenceService _preferenceService;
        private readonly ILoggerService _logger;
        private readonly IAuthenticationService _authService;

        private string _apiBaseUrl = string.Empty;
        private int _syncInterval;
        private bool _backgroundSyncEnabled;
        private bool _wifiOnly;
        private bool _useMobileData;
        private bool _notificationsEnabled;
        private bool _loggingEnabled;
        private bool _autoRetry;
        private bool _darkMode;
        private string _customRecordingPath = string.Empty;

        // Properties
        public string ApiBaseUrl { get => _apiBaseUrl; set { SetProperty(ref _apiBaseUrl, value); SaveSetting(AppConstants.PrefKeyApiBaseUrl, value); } }
        public string CustomRecordingPath { get => _customRecordingPath; set { SetProperty(ref _customRecordingPath, value); SaveSetting(AppConstants.PrefKeyCustomRecordingPath, value); } }
        public int SyncInterval { get => _syncInterval; set { SetProperty(ref _syncInterval, value); SaveSetting(AppConstants.PrefKeySyncInterval, value); } }
        public bool BackgroundSyncEnabled { get => _backgroundSyncEnabled; set { SetProperty(ref _backgroundSyncEnabled, value); SaveSetting(AppConstants.PrefKeyBackgroundSyncEnabled, value); } }
        public bool WifiOnly { get => _wifiOnly; set { SetProperty(ref _wifiOnly, value); SaveSetting(AppConstants.PrefKeyWifiOnly, value); } }
        public bool UseMobileData { get => _useMobileData; set { SetProperty(ref _useMobileData, value); SaveSetting(AppConstants.PrefKeyUseMobileData, value); } }
        public bool NotificationsEnabled { get => _notificationsEnabled; set { SetProperty(ref _notificationsEnabled, value); SaveSetting(AppConstants.PrefKeyNotificationsEnabled, value); } }
        public bool LoggingEnabled { get => _loggingEnabled; set { SetProperty(ref _loggingEnabled, value); SaveSetting(AppConstants.PrefKeyLoggingEnabled, value); } }
        public bool AutoRetry { get => _autoRetry; set { SetProperty(ref _autoRetry, value); SaveSetting(AppConstants.PrefKeyAutoRetry, value); } }
        public bool DarkMode { get => _darkMode; set { SetProperty(ref _darkMode, value); SaveSetting(AppConstants.PrefKeyDarkMode, value); } }

        /// <summary>Event raised when the user requests logout.</summary>
        public event EventHandler? LogoutRequested;

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsViewModel"/>.
        /// </summary>
        public SettingsViewModel(IPreferenceService preferenceService, IAuthenticationService authService, ILoggerService logger)
        {
            _preferenceService = preferenceService;
            _authService = authService;
            _logger = logger;
            Title = "Settings";
        }

        /// <summary>
        /// Loads all current settings values from preferences.
        /// </summary>
        public void LoadSettings()
        {
            _apiBaseUrl = _preferenceService.GetString(AppConstants.PrefKeyApiBaseUrl, AppConstants.DefaultApiBaseUrl);
            _customRecordingPath = _preferenceService.GetString(AppConstants.PrefKeyCustomRecordingPath, string.Empty);
            _syncInterval = _preferenceService.GetInt(AppConstants.PrefKeySyncInterval, AppConstants.DefaultSyncIntervalMinutes);
            _backgroundSyncEnabled = _preferenceService.GetBool(AppConstants.PrefKeyBackgroundSyncEnabled, true);
            _wifiOnly = _preferenceService.GetBool(AppConstants.PrefKeyWifiOnly, false);
            _useMobileData = _preferenceService.GetBool(AppConstants.PrefKeyUseMobileData, true);
            _notificationsEnabled = _preferenceService.GetBool(AppConstants.PrefKeyNotificationsEnabled, true);
            _loggingEnabled = _preferenceService.GetBool(AppConstants.PrefKeyLoggingEnabled, true);
            _autoRetry = _preferenceService.GetBool(AppConstants.PrefKeyAutoRetry, true);
            _darkMode = _preferenceService.GetBool(AppConstants.PrefKeyDarkMode, false);

            // Raise all property changed
            OnPropertyChanged(nameof(ApiBaseUrl));
            OnPropertyChanged(nameof(CustomRecordingPath));
            OnPropertyChanged(nameof(SyncInterval));
            OnPropertyChanged(nameof(BackgroundSyncEnabled));
            OnPropertyChanged(nameof(WifiOnly));
            OnPropertyChanged(nameof(UseMobileData));
            OnPropertyChanged(nameof(NotificationsEnabled));
            OnPropertyChanged(nameof(LoggingEnabled));
            OnPropertyChanged(nameof(AutoRetry));
            OnPropertyChanged(nameof(DarkMode));
        }

        /// <summary>
        /// Persists a string setting value.
        /// </summary>
        private void SaveSetting(string key, string value)
        {
            _preferenceService.SetString(key, value);
        }

        /// <summary>
        /// Persists an integer setting value.
        /// </summary>
        private void SaveSetting(string key, int value)
        {
            _preferenceService.SetInt(key, value);
        }

        /// <summary>
        /// Persists a boolean setting value.
        /// </summary>
        private void SaveSetting(string key, bool value)
        {
            _preferenceService.SetBool(key, value);
        }

        /// <summary>
        /// Logs out the user and raises the LogoutRequested event.
        /// </summary>
        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
            _logger.Info("SettingsVM", "User logged out.");
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
