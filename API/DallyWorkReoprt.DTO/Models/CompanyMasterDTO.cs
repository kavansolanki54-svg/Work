using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models
{
    public class CompanyMasterDTO
    {
        public CompanyMasterDTO() { }

        public CompanyMasterDTO(SignUpViewModel model)
        {
            CompanyName = model.CompanyName;
            Email = model.Email;
        }

        [Key]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string PasswordHash { get; set; } = string.Empty;

        public byte? IsEmailVerified { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(10, ErrorMessage = "Phone Number cannot exceed 10 characters.")]
        public string PhoneNo { get; set; } = string.Empty;

        public byte? IsMobileNoVerified { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? PreferredSubDomain { get; set; }

        [StringLength(300)]
        public string? FullAddress { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        public int StateId { get; set; }

        [StringLength(100)]
        public string? CityName { get; set; }

        public string? Pincode { get; set; }

        public byte ActiveStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid Guids { get; set; }

        [StringLength(200)]
        public string? LogoUrl { get; set; }

    }

    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(300)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [StringLength(300)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}

