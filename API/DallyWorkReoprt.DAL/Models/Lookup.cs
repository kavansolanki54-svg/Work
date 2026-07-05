using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("Lookup")]
public partial class Lookup
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

    [InverseProperty("Gender")]
    public virtual ICollection<EmployeeMaster> EmployeeMasters { get; set; } = new List<EmployeeMaster>();

    [InverseProperty("RoleType")]
    public virtual ICollection<RoleMaster> RoleMasters { get; set; } = new List<RoleMaster>();

    [ForeignKey("TypeId")]
    [InverseProperty("Lookups")]
    public virtual LookupType Type { get; set; } = null!;
}
