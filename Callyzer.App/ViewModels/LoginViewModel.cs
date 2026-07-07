using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Extensions;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Login page.
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IPreferenceService _prefs;
        private readonly ILoggerService _logger;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isPasswordVisible;

        [ObservableProperty]
        private string _apiBaseUrl;

        public LoginViewModel(IPreferenceService prefs, ILoggerService logger)
        {
            _prefs = prefs;
            _logger = logger;
            Title = "Sign In";
            _apiBaseUrl = _prefs.GetString(Constants.AppConstants.PrefKeyApiBaseUrl,
                Constants.AppConstants.DefaultApiBaseUrl);
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter your email and password.";
                    return;
                }

                _logger.Info("LoginVM", $"Login attempt for {Email.MaskEmail()}");

                // Save API base URL preference
                _prefs.SetString(Constants.AppConstants.PrefKeyApiBaseUrl, ApiBaseUrl);

                // TODO: Wire up AuthenticationService.LoginAsync(email, password)
                // On success: navigate to AppShell
                // On failure: show error

                await Task.Delay(100); // Placeholder
            }, "Login failed");
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
