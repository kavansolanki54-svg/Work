using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models
{
    public class LookupTypeDTO
    {
        [Key]
        public short Id { get; set; }

        [StringLength(200)]
        public string TypeName { get; set; } = null!;

        [StringLength(200)]
        public string? Icon { get; set; }

        public bool ActiveStatus { get; set; }

        public List<LookupDTO> Lookups { get; set; } = new List<LookupDTO>();
    }
}

