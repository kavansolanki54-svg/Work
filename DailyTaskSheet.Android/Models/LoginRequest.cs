using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Login request payload matching the backend's LoginDTO.
    /// Sent to POST /api/Auth/login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>Gets or sets the user's email address.</summary>
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's password (plaintext over HTTPS).</summary>
        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
