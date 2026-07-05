using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("CountryMaster")]
public partial class CountryMaster
{
    [Key]
    [Column("CountryID")]
    public int CountryId { get; set; }

    [StringLength(50)]
    public string CountryName { get; set; } = null!;

    [StringLength(10)]
    public string? CountryCode { get; set; }

    [StringLength(10)]
    public string? CurrencyCode { get; set; }

    [StringLength(10)]
    public string? CurrencySymbole { get; set; }

    public byte ActiveStatus { get; set; }

    public Guid Guids { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<CompanyMaster> CompanyMasters { get; set; } = new List<CompanyMaster>();
}
