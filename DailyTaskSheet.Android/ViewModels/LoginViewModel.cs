using System;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Login screen.
    /// Handles email/password validation and authentication via the AuthenticationService.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly ILoggerService _logger;
        private string _email = string.Empty;
        private string _password = string.Empty;

        /// <summary>Gets or sets the email input.</summary>
        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); ClearError(); }
        }

        /// <summary>Gets or sets the password input.</summary>
        public string Password
        {
            get => _password;
            set { SetProperty(ref _password, value); ClearError(); }
        }

        /// <summary>Event raised when login succeeds and the user should navigate to Dashboard.</summary>
        public event EventHandler? LoginSucceeded;

        /// <summary>
        /// Initializes a new instance of <see cref="LoginViewModel"/>.
        /// </summary>
        public LoginViewModel(IAuthenticationService authService, ILoggerService logger)
        {
            _authService = authService;
            _logger = logger;
            Title = "Login";
        }

        /// <summary>
        /// Validates inputs and attempts login.
        /// </summary>
        public async Task LoginAsync()
        {
            if (IsBusy) return;

            ClearError();

            // Validate
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password.";
                return;
            }

            if (!Email.Contains("@"))
            {
                ErrorMessage = "Please enter a valid email address.";
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _authService.LoginAsync(Email.Trim(), Password, CancellationToken.None);

                if (result.Success)
                {
                    _logger.Info("LoginVM", "Login successful, navigating to dashboard.");
                    LoginSucceeded?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ErrorMessage = result.Message ?? "Login failed. Please check your credentials.";
                }
            }
            catch (Exceptions.AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
                _logger.Error("LoginVM", "Authentication failed", ex);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to connect to server. Please check your internet connection.";
                _logger.Error("LoginVM", "Login error", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
