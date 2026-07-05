using System;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Exceptions;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Handles user authentication, token lifecycle, and credential storage.
    /// Uses the ApiClient for HTTP calls and PreferenceService for secure storage.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiClient _apiClient;
        private readonly IPreferenceService _preferenceService;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationService"/>.
        /// </summary>
        public AuthenticationService(
            IApiClient apiClient,
            IPreferenceService preferenceService,
            IUserRepository userRepository,
            ILoggerService logger)
        {
            _apiClient = apiClient;
            _preferenceService = preferenceService;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsAuthenticated =>
            _preferenceService.GetBool(AppConstants.PrefKeyIsLoggedIn, false) &&
            !string.IsNullOrEmpty(_preferenceService.GetString(AppConstants.PrefKeyAccessToken));

        /// <inheritdoc />
        public bool IsTokenExpired
        {
            get
            {
                long expiryTicks = _preferenceService.GetLong(AppConstants.PrefKeyTokenExpiry, 0);
                if (expiryTicks == 0) return true;
                return DateTime.UtcNow.Ticks >= expiryTicks;
            }
        }

        /// <inheritdoc />
        public async Task<ApiResult<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.Info("AuthService", $"Attempting login for: {email}");

                var request = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                var result = await _apiClient.PostAnonymousAsync<LoginRequest, LoginResponse>(
                    ApiEndpoints.Login, request, cancellationToken);

                if (result.Success && result.Data != null)
                {
                    await StoreCredentialsAsync(result.Data, cancellationToken);
                    _logger.Info("AuthService", "Login successful.");
                }
                else
                {
                    _logger.Warning("AuthService", $"Login failed: {result.Message}");
                }

                return result;
            }
            catch (ApiException ex)
            {
                _logger.Error("AuthService", "Login API error", ex);
                throw new AuthenticationException(
                    ex.Message,
                    ex.IsAuthenticationError ? AuthFailureReason.InvalidCredentials : AuthFailureReason.ServerError,
                    ex);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.Error("AuthService", "Login error", ex);
                throw new AuthenticationException(
                    "Unable to connect to server. Please check your internet connection.",
                    AuthFailureReason.NetworkError,
                    ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                string accessToken = _preferenceService.GetString(AppConstants.PrefKeyAccessToken);
                string refreshToken = _preferenceService.GetString(AppConstants.PrefKeyRefreshToken);

                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.Warning("AuthService", "No refresh token available.");
                    return false;
                }

                var request = new RefreshTokenRequest
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                var result = await _apiClient.PostAnonymousAsync<RefreshTokenRequest, RefreshTokenResponse>(
                    ApiEndpoints.RefreshToken, request, cancellationToken);

                if (result.Success && result.Data != null)
                {
                    _preferenceService.SetString(AppConstants.PrefKeyAccessToken, result.Data.AccessToken);
                    _preferenceService.SetString(AppConstants.PrefKeyRefreshToken, result.Data.RefreshToken);
                    // Set new expiry: 6 hours from now (matching backend configuration)
                    _preferenceService.SetLong(AppConstants.PrefKeyTokenExpiry,
                        DateTime.UtcNow.AddHours(6).Ticks);

                    _logger.Info("AuthService", "Token refreshed successfully.");
                    return true;
                }

                _logger.Warning("AuthService", $"Token refresh failed: {result.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error("AuthService", "Token refresh error", ex);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task LogoutAsync()
        {
            try
            {
                _logger.Info("AuthService", "Logging out user.");
                _preferenceService.ClearAll();
                _logger.Info("AuthService", "Credentials cleared.");
            }
            catch (Exception ex)
            {
                _logger.Error("AuthService", "Error during logout", ex);
            }
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public string? GetAccessToken() => _preferenceService.GetString(AppConstants.PrefKeyAccessToken);

        /// <inheritdoc />
        public int GetEmployeeId() => _preferenceService.GetInt(AppConstants.PrefKeyEmployeeId);

        /// <inheritdoc />
        public int GetCompanyId() => _preferenceService.GetInt(AppConstants.PrefKeyCompanyId);

        /// <inheritdoc />
        public string GetUserName() => _preferenceService.GetString(AppConstants.PrefKeyUserName, "User");

        /// <summary>
        /// Stores authentication credentials securely after successful login.
        /// </summary>
        private async Task StoreCredentialsAsync(LoginResponse loginResponse, CancellationToken cancellationToken)
        {
            _preferenceService.SetString(AppConstants.PrefKeyAccessToken, loginResponse.AccessToken);
            _preferenceService.SetString(AppConstants.PrefKeyRefreshToken, loginResponse.RefreshToken);
            _preferenceService.SetLong(AppConstants.PrefKeyTokenExpiry, DateTime.UtcNow.AddHours(6).Ticks);
            _preferenceService.SetBool(AppConstants.PrefKeyIsLoggedIn, true);

            if (loginResponse.User != null)
            {
                _preferenceService.SetInt(AppConstants.PrefKeyEmployeeId, loginResponse.User.EmployeeId);
                _preferenceService.SetInt(AppConstants.PrefKeyCompanyId, loginResponse.User.CompanyId);
                _preferenceService.SetInt(AppConstants.PrefKeyRoleId, loginResponse.User.RoleId);
                _preferenceService.SetString(AppConstants.PrefKeyUserName, loginResponse.User.UserName);
                _preferenceService.SetString(AppConstants.PrefKeyEmail, loginResponse.User.Email);
                _preferenceService.SetString(AppConstants.PrefKeyRoleName, loginResponse.User.RoleName);

                // Store user in SQLite for offline access
                var userModel = new UserModel
                {
                    EmployeeId = loginResponse.User.EmployeeId,
                    Email = loginResponse.User.Email,
                    UserName = loginResponse.User.UserName,
                    CompanyId = loginResponse.User.CompanyId,
                    RoleId = loginResponse.User.RoleId,
                    RoleName = loginResponse.User.RoleName,
                    RoleType = loginResponse.User.RoleType,
                    IsTenant = loginResponse.User.IsTenant,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.UpsertAsync(userModel, cancellationToken);
            }
        }
    }
}
