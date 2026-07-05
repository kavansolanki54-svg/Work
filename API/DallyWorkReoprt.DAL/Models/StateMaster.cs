using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("StateMaster")]
public partial class StateMaster
{
    [Key]
    [Column("StateID")]
    public int StateId { get; set; }

    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(50)]
    public string StateName { get; set; } = null!;

    public byte ActiveStatus { get; set; }

    public Guid Guids { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string StateCode { get; set; } = null!;

    [InverseProperty("State")]
    public virtual ICollection<CompanyMaster> CompanyMasters { get; set; } = new List<CompanyMaster>();
}
