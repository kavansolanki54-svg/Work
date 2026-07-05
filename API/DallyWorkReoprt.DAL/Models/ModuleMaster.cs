using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("ModuleMaster")]
[Index("CompanyId", Name = "IX_ModuleMaster_CompanyID")]
public partial class ModuleMaster
{
    [Key]
    [Column("ModuleID")]
    public int ModuleId { get; set; }

    [StringLength(200)]
    public string ModuleName { get; set; } = null!;

    [Column("CompanyID")]
    public int CompanyId { get; set; }

    public byte ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    [Unicode(false)]
    public string CreatedById { get; set; } = null!;

    [Column("ModifiedByID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    [Column("ParentModuleID")]
    public int? ParentModuleId { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("ModuleMasters")]
    public virtual CompanyMaster Company { get; set; } = null!;

    [InverseProperty("ParentModule")]
    public virtual ICollection<ModuleMaster> InverseParentModule { get; set; } = new List<ModuleMaster>();

    [ForeignKey("ParentModuleId")]
    [InverseProperty("InverseParentModule")]
    public virtual ModuleMaster? ParentModule { get; set; }
}
