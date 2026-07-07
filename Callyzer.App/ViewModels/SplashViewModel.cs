using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Interfaces;

namespace Callyzer.App.ViewModels
{
    public partial class SplashViewModel : BaseViewModel
    {
        private readonly ICallLogPlatformService _callLogService;
        private readonly IPreferenceService _preferenceService;
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        private string _statusMessage = "Loading Callyzer...";

        [ObservableProperty]
        private bool _isPermissionContainerVisible;

        public SplashViewModel(
            ICallLogPlatformService callLogService,
            IPreferenceService preferenceService,
            IAnalyticsService analyticsService)
        {
            _callLogService = callLogService;
            _preferenceService = preferenceService;
            _analyticsService = analyticsService;
        }

        [RelayCommand]
        private async Task InitializeAppAsync()
        {
            try
            {
                // Give the UI a moment to render and animate the logo
                await Task.Delay(1500);

                // 1. Check Auth (Mocked for now, assuming logged in if token exists)
                var hasToken = !string.IsNullOrEmpty(_preferenceService.GetString("AuthToken", "mock_token"));
                
                if (!hasToken)
                {
                    NavigateToMainApp(false);
                    return;
                }

                // 2. Check Permissions
                if (_callLogService.IsCallLogAccessSupported)
                {
                    if (!_callLogService.HasRequiredPermissions)
                    {
                        StatusMessage = "We need access to your call logs to provide analytics.";
                        IsPermissionContainerVisible = true;
                        return; // Wait for user to tap the grant button
                    }
                }

                // 3. Pre-warm analytics cache
                StatusMessage = "Preparing your dashboard...";
                await _analyticsService.GetSummaryAsync(Enums.TimePeriodEnum.Daily);

                // 4. Navigate to main app
                NavigateToMainApp(true);
            }
            catch (Exception ex)
            {
                StatusMessage = "An error occurred during startup.";
                System.Diagnostics.Debug.WriteLine($"Startup Error: {ex}");
            }
        }

        [RelayCommand]
        private async Task GrantPermissionsAsync()
        {
            var granted = await _callLogService.RequestPermissionsAsync();
            if (granted)
            {
                IsPermissionContainerVisible = false;
                StatusMessage = "Preparing your dashboard...";
                await _analyticsService.GetSummaryAsync(Enums.TimePeriodEnum.Daily);
                NavigateToMainApp(true);
            }
            else
            {
                StatusMessage = "Permissions denied. Callyzer cannot function without call log access.";
            }
        }

        private void NavigateToMainApp(bool isAuthenticated)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current != null)
                {
                    // Swap the main page to the AppShell
                    Application.Current.MainPage = new AppShell();
                    
                    if (!isAuthenticated)
                    {
                        Shell.Current.GoToAsync("//login");
                    }
                }
            });
        }
    }
}
