using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("CompanyMaster")]
public partial class CompanyMaster
{
    [Key]
    [Column("CompanyID")]
    public int CompanyId { get; set; }

    [StringLength(200)]
    public string CompanyName { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    public byte IsEmailVerified { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? PhoneNo { get; set; }

    public byte IsMobileNoVerified { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Website { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? PreferredSubDomain { get; set; }

    [StringLength(300)]
    public string? FullAddress { get; set; }

    [Column("CountryID")]
    public int? CountryId { get; set; }

    [Column("StateID")]
    public int? StateId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CityName { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Pincode { get; set; }

    public byte ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public Guid Guids { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? LogoUrl { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<ClientMaster> ClientMasters { get; set; } = new List<ClientMaster>();

    [ForeignKey("CountryId")]
    [InverseProperty("CompanyMasters")]
    public virtual CountryMaster Company { get; set; } = null!;

    [InverseProperty("Company")]
    public virtual ICollection<ModuleMaster> ModuleMasters { get; set; } = new List<ModuleMaster>();

    [InverseProperty("Company")]
    public virtual ICollection<ProjectMaster> ProjectMasters { get; set; } = new List<ProjectMaster>();

    [InverseProperty("Company")]
    public virtual ICollection<RoleMaster> RoleMasters { get; set; } = new List<RoleMaster>();

    [ForeignKey("StateId")]
    [InverseProperty("CompanyMasters")]
    public virtual StateMaster? State { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<StatusMaster> StatusMasters { get; set; } = new List<StatusMaster>();
}
