using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("RoleMaster")]
public partial class RoleMaster
{
    [Key]
    public int RoleMasterId { get; set; }

    public int CompanyId { get; set; }

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    public int RoleTypeId { get; set; }

    [StringLength(300)]
    public string? Descriptions { get; set; }

    public byte ActiveStatus { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    public string CreatedById { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("ModifiedByID")]
    [StringLength(100)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("RoleMasters")]
    public virtual CompanyMaster Company { get; set; } = null!;

    [InverseProperty("RoleMaster")]
    public virtual ICollection<EmployeeMaster> EmployeeMasters { get; set; } = new List<EmployeeMaster>();

    [InverseProperty("RoleMaster")]
    public virtual ICollection<RoleMasterSoftwareModule> RoleMasterSoftwareModules { get; set; } = new List<RoleMasterSoftwareModule>();

    [ForeignKey("RoleTypeId")]
    [InverseProperty("RoleMasters")]
    public virtual Lookup RoleType { get; set; } = null!;
}
