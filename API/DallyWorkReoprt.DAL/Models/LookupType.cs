using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("LookupType")]
public partial class LookupType
{
    [Key]
    public short Id { get; set; }

    [StringLength(200)]
    public string TypeName { get; set; } = null!;

    [StringLength(200)]
    public string? Icon { get; set; }

    public bool ActiveStatus { get; set; }

    [InverseProperty("Type")]
    public virtual ICollection<Lookup> Lookups { get; set; } = new List<Lookup>();
}
