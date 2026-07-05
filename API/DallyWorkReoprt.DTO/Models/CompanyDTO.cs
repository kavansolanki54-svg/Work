using Microsoft.AspNetCore.Http;

namespace DallyWorkReoprt.DTO.Models
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullAddress { get; set; }
        public string? CityName { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public string? Pincode { get; set; }
        public string? LogoUrl { get; set; }

        // Fields for file uploads
        public IFormFile? LogoFile { get; set; }
    }
}
