using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Response payload from the token refresh endpoint.
    /// Contains new access and refresh tokens.
    /// </summary>
    public class RefreshTokenResponse
    {
        /// <summary>Gets or sets the new JWT access token.</summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the new refresh token.</summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the user info (may be returned again).</summary>
        [JsonProperty("user")]
        public UserBasicInfo? User { get; set; }
    }
}
