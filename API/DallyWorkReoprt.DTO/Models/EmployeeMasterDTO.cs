using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models
{
    public class EmployeeMasterDTO
    {
        [Key]
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public int DefaultBreakDuration { get; set; } = 30;

        [Required(ErrorMessage = "Role is required.")]
        [Display(Name = "Role")]
        public int RoleMasterId { get; set; }

        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Middle Name cannot exceed 100 characters.")]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "Designation cannot exceed 100 characters.")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(200, ErrorMessage = "Email Address cannot exceed 200 characters.")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(300, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        public string Passwords { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Mobile Number cannot exceed 50 characters.")]
        [Display(Name = "Mobile Number")]
        public string? MobileNo { get; set; }

        [StringLength(100, ErrorMessage = "Employee Photo File name cannot exceed 100 characters.")]
        public string? EmployeePhotoFile { get; set; }

        [Display(Name = "Allow Login")]
        public bool IsAllowLogin { get; set; }

        [Display(Name = "Employee Code")]
        public int? EmployeeCode { get; set; }
        public DateOnly? BirthDate { get; set; }

        public DateOnly? DateOfJoining { get; set; }

        [Display(Name = "Gender")]
        public int? GenderId { get; set; }

        public string? Address { get; set; }

        public byte SignInAttempt { get; set; }

        public string? EmployeeName { get; set; }

        public byte ActiveStatus { get; set; }

        [StringLength(20, ErrorMessage = "Created By cannot exceed 20 characters.")]
        public string CreatedById { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        [StringLength(20, ErrorMessage = "Modified By cannot exceed 20 characters.")]
        public string? ModifiedById { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public Guid Guids { get; set; }
        public bool Tenant { get; set; }
        public LookupDTO? Gender { get; set; }

        public CompanyMasterDTO? Company { get; set; }

        public RoleMasterDTO? RoleMaster { get; set; }

    }
}

