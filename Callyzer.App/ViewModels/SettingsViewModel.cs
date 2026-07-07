using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Interfaces;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Settings page.
    /// </summary>
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly IPreferenceService _prefs;
        private readonly ILoggerService _logger;

        [ObservableProperty]
        private int _syncIntervalMinutes;

        [ObservableProperty]
        private bool _backgroundSyncEnabled;

        [ObservableProperty]
        private bool _wifiOnlySync;

        [ObservableProperty]
        private bool _notificationsEnabled;

        [ObservableProperty]
        private bool _darkModeEnabled;

        [ObservableProperty]
        private string _apiBaseUrl = string.Empty;

        [ObservableProperty]
        private string _databaseSizeDisplay = string.Empty;

        [ObservableProperty]
        private int _totalCallLogs;

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        public SettingsViewModel(IPreferenceService prefs, ILoggerService logger)
        {
            _prefs = prefs;
            _logger = logger;
            Title = "Settings";
        }

        [RelayCommand]
        private Task LoadSettingsAsync()
        {
            SyncIntervalMinutes = _prefs.GetInt(Constants.AppConstants.PrefKeySyncInterval,
                Constants.AppConstants.DefaultSyncIntervalMinutes);
            BackgroundSyncEnabled = _prefs.GetBool(Constants.AppConstants.PrefKeyBackgroundSyncEnabled, true);
            WifiOnlySync = _prefs.GetBool(Constants.AppConstants.PrefKeyWifiOnly, false);
            NotificationsEnabled = _prefs.GetBool(Constants.AppConstants.PrefKeyNotificationsEnabled, true);
            DarkModeEnabled = _prefs.GetBool(Constants.AppConstants.PrefKeyDarkMode, false);
            ApiBaseUrl = _prefs.GetString(Constants.AppConstants.PrefKeyApiBaseUrl,
                Constants.AppConstants.DefaultApiBaseUrl);
            UserName = _prefs.GetString(Constants.AppConstants.PrefKeyUserName);
            Email = _prefs.GetString(Constants.AppConstants.PrefKeyEmail);

            _logger.Info("SettingsVM", "Settings loaded");
            return Task.CompletedTask;
        }

        [RelayCommand]
        private void SaveSettings()
        {
            _prefs.SetInt(Constants.AppConstants.PrefKeySyncInterval, SyncIntervalMinutes);
            _prefs.SetBool(Constants.AppConstants.PrefKeyBackgroundSyncEnabled, BackgroundSyncEnabled);
            _prefs.SetBool(Constants.AppConstants.PrefKeyWifiOnly, WifiOnlySync);
            _prefs.SetBool(Constants.AppConstants.PrefKeyNotificationsEnabled, NotificationsEnabled);
            _prefs.SetBool(Constants.AppConstants.PrefKeyDarkMode, DarkModeEnabled);
            _prefs.SetString(Constants.AppConstants.PrefKeyApiBaseUrl, ApiBaseUrl);

            _logger.Info("SettingsVM", "Settings saved");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            _prefs.SetBool(Constants.AppConstants.PrefKeyIsLoggedIn, false);
            _prefs.Remove(Constants.AppConstants.PrefKeyAccessToken);
            _prefs.Remove(Constants.AppConstants.PrefKeyRefreshToken);

            _logger.Info("SettingsVM", "User logged out");
            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        private async Task NavigateToBackupAsync()
        {
            await Shell.Current.GoToAsync("backuprestore");
        }

        [RelayCommand]
        private async Task NavigateToExportAsync()
        {
            await Shell.Current.GoToAsync("export");
        }

        [RelayCommand]
        private async Task NavigateToSyncHistoryAsync()
        {
            await Shell.Current.GoToAsync("synchistory");
        }
    }
}
