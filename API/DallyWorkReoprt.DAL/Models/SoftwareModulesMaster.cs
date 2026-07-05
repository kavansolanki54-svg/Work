using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("SoftwareModulesMaster")]
public partial class SoftwareModulesMaster
{
    [Key]
    [Column("SoftwareModulesMasterID")]
    public int SoftwareModulesMasterId { get; set; }

    public int? ParentId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string ModulesName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string ControllersName { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string ActionName { get; set; } = null!;

    public bool? HasCreate { get; set; }

    [Column("FullURL")]
    [StringLength(500)]
    [Unicode(false)]
    public string? FullUrl { get; set; }

    public int? DisplayOrder { get; set; }

    public byte ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CrDate { get; set; }

    public Guid Guids { get; set; }

    [StringLength(200)]
    public string? ImagePath { get; set; }

    [StringLength(200)]
    public string? Icon { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool? IsNew { get; set; }

    [Column("ExternalURI")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? ExternalUri { get; set; }

    public byte DisplayLevel { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<SoftwareModulesMaster> InverseParent { get; set; } = new List<SoftwareModulesMaster>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual SoftwareModulesMaster? Parent { get; set; }

    [InverseProperty("SoftwareModulesMaster")]
    public virtual ICollection<RoleMasterSoftwareModule> RoleMasterSoftwareModules { get; set; } = new List<RoleMasterSoftwareModule>();
}
