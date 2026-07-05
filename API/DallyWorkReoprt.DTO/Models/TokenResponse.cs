namespace DallyWorkReoprt.DTO.Models
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public UserBasicDTO User { get; set; } = null!;
    }
}

