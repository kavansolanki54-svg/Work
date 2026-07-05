using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Request payload for refreshing an expired JWT token.
    /// Sent to POST /api/Auth/RefreshToken.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>Gets or sets the expired access token.</summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the refresh token.</summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
