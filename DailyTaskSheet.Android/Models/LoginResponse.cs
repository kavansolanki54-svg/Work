using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Login response payload matching the backend's TokenResponse structure.
    /// Returned from POST /api/Auth/login wrapped in ApiResult&lt;LoginResponse&gt;.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>Gets or sets the JWT access token.</summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the refresh token for obtaining new access tokens.</summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the authenticated user's basic information.</summary>
        [JsonProperty("user")]
        public UserBasicInfo User { get; set; } = new UserBasicInfo();
    }

    /// <summary>
    /// User basic information returned in the login response.
    /// Maps to the backend's UserBasicDTO.
    /// </summary>
    public class UserBasicInfo
    {
        /// <summary>Gets or sets the employee ID.</summary>
        [JsonProperty("employeeID")]
        public int EmployeeId { get; set; }

        /// <summary>Gets or sets the employee/user name.</summary>
        [JsonProperty("userName")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's email address.</summary>
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's role name.</summary>
        [JsonProperty("roleName")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's role type.</summary>
        [JsonProperty("roleType")]
        public string RoleType { get; set; } = string.Empty;

        /// <summary>Gets or sets the user's role ID.</summary>
        [JsonProperty("roleId")]
        public int RoleId { get; set; }

        /// <summary>Gets or sets the user's company ID.</summary>
        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        /// <summary>Gets or sets whether this user is a tenant administrator.</summary>
        [JsonProperty("isTenant")]
        public bool IsTenant { get; set; }
    }
}
