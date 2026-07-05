using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("ProjectMaster")]
[Index("CompanyId", Name = "IX_ProjectMaster_CompanyID")]
public partial class ProjectMaster
{
    [Key]
    [Column("ProjectID")]
    public int ProjectId { get; set; }

    [StringLength(200)]
    public string ProjectName { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string? ProjectColor { get; set; }

    [Column("CompanyID")]
    public int CompanyId { get; set; }

    public byte ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    public string CreatedById { get; set; } = null!;

    [Column("ModifiedByID")]
    [StringLength(100)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("ProjectMasters")]
    public virtual CompanyMaster Company { get; set; } = null!;
}
