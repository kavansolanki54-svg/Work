using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models
{
    public class LookupDTO
    {
        [Key]
        public int Id { get; set; }

        public short TypeId { get; set; }

        [StringLength(200)]
        public string LookupName { get; set; } = null!;

        [StringLength(200)]
        public string? Icon { get; set; }

        public bool ActiveStatus { get; set; }

        public int DisplayOrder { get; set; }

        public List<RoleMasterDTO> RoleMasters { get; set; } = new List<RoleMasterDTO>();

        public LookupTypeDTO Type { get; set; } = null!;
    }
}

