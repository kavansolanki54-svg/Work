using System;

namespace DailyTaskSheet.App.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when an authentication operation fails.
    /// Covers token expiry, invalid credentials, and account-related issues.
    /// </summary>
    public class AuthenticationException : Exception
    {
        /// <summary>Gets the type of authentication failure.</summary>
        public AuthFailureReason Reason { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationException"/>.
        /// </summary>
        /// <param name="message">Human-readable error message.</param>
        /// <param name="reason">The specific reason for the authentication failure.</param>
        /// <param name="innerException">Optional inner exception.</param>
        public AuthenticationException(
            string message,
            AuthFailureReason reason = AuthFailureReason.Unknown,
            Exception? innerException = null)
            : base(message, innerException)
        {
            Reason = reason;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"AuthenticationException: {Message} | Reason: {Reason}";
        }
    }

    /// <summary>
    /// Enumerates the specific reasons for an authentication failure.
    /// </summary>
    public enum AuthFailureReason
    {
        /// <summary>Unknown or unspecified reason.</summary>
        Unknown = 0,

        /// <summary>The provided credentials were invalid.</summary>
        InvalidCredentials = 1,

        /// <summary>The JWT access token has expired.</summary>
        TokenExpired = 2,

        /// <summary>The refresh token is invalid or has been revoked.</summary>
        RefreshTokenInvalid = 3,

        /// <summary>The user's account has been disabled by an administrator.</summary>
        AccountDisabled = 4,

        /// <summary>Network failure prevented authentication.</summary>
        NetworkError = 5,

        /// <summary>The server returned an unexpected response.</summary>
        ServerError = 6
    }
}
