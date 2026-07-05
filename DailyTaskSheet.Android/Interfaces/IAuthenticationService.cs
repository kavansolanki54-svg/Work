using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for user authentication operations.
    /// Handles login, token management, and session lifecycle.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>Authenticates a user with email and password.</summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The login response with tokens and user info.</returns>
        Task<ApiResult<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

        /// <summary>Refreshes the JWT access token using the stored refresh token.</summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the token was refreshed successfully.</returns>
        Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>Logs out the current user and clears all stored credentials.</summary>
        /// <returns>A completed task.</returns>
        Task LogoutAsync();

        /// <summary>Gets whether the user is currently authenticated.</summary>
        bool IsAuthenticated { get; }

        /// <summary>Gets whether the current token needs refreshing.</summary>
        bool IsTokenExpired { get; }

        /// <summary>Gets the current JWT access token.</summary>
        string? GetAccessToken();

        /// <summary>Gets the currently stored employee ID.</summary>
        int GetEmployeeId();

        /// <summary>Gets the currently stored company ID.</summary>
        int GetCompanyId();

        /// <summary>Gets the currently stored company name.</summary>
        string GetCompanyName();

        /// <summary>Gets the currently stored user name.</summary>
        string GetUserName();
    }
}
