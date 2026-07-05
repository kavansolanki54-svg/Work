using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models
{
    public class RoleMasterDTO
    {
        [Key]
        public int RoleMasterId { get; set; }

        public int CompanyId { get; set; }
        [Required(ErrorMessage = "Role Name is required.")]
        [StringLength(100, ErrorMessage = "Role Name cannot exceed 100 characters.")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Role Type")]
        public int RoleTypeId { get; set; }

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        [Display(Name = "Description")]
        public string? Descriptions { get; set; }

        public byte ActiveStatus { get; set; }

        [StringLength(20, ErrorMessage = "Created By cannot exceed 20 characters.")]
        public string CreatedById { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        [StringLength(20, ErrorMessage = "Modified By cannot exceed 20 characters.")]
        public string? ModifiedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid Guids { get; set; }

        public LookupDTO? RoleType { get; set; }
    }
}

