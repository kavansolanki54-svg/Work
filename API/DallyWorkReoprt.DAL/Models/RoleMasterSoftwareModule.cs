using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("RoleMasterSoftwareModules")]
public partial class RoleMasterSoftwareModule
{
    [Key]
    [Column("RoleMasterSoftwareModulesID")]
    public int RoleMasterSoftwareModulesId { get; set; }

    [Column("RoleMasterID")]
    public int RoleMasterId { get; set; }

    [Column("SoftwareModulesMasterID")]
    public int SoftwareModulesMasterId { get; set; }

    public bool? View { get; set; }

    public bool? Add { get; set; }

    public bool? Edit { get; set; }

    public bool? Delete { get; set; }

    public byte ActiveStatus { get; set; }

    [Column("CreatedByID")]
    [StringLength(20)]
    [Unicode(false)]
    public string CreatedById { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("ModifiedByID")]
    [StringLength(20)]
    [Unicode(false)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    [ForeignKey("RoleMasterId")]
    [InverseProperty("RoleMasterSoftwareModules")]
    public virtual RoleMaster RoleMaster { get; set; } = null!;

    [ForeignKey("SoftwareModulesMasterId")]
    [InverseProperty("RoleMasterSoftwareModules")]
    public virtual SoftwareModulesMaster SoftwareModulesMaster { get; set; } = null!;
}
