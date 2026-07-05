using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("StatusMaster")]
[Index("CompanyId", Name = "IX_StatusMaster_CompanyID")]
public partial class StatusMaster
{
    [Key]
    [Column("StatusID")]
    public int StatusId { get; set; }

    [StringLength(200)]
    public string StatusName { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string? StatusColor { get; set; }

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
    [InverseProperty("StatusMasters")]
    public virtual CompanyMaster Company { get; set; } = null!;
}
