using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("WorkEntries")]
public partial class WorkEntry
{
    [Key]
    public int Id { get; set; }

    public int WorkReportId { get; set; }

    public int SrNo { get; set; }

    [StringLength(500)]
    public string Title { get; set; } = null!;

    public int ProjectId { get; set; }

    public int StatusId { get; set; }

    public int? ModuleId { get; set; }

    public string? Description { get; set; }

    public byte ActiveStatus { get; set; }

    [Column("CreatedByID")]
    [StringLength(20)]
    public string CreatedById { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("ModifiedBYID")]
    [StringLength(20)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    [ForeignKey("WorkReportId")]
    public virtual WorkReport WorkReport { get; set; } = null!;

    [ForeignKey("ProjectId")]
    public virtual ProjectMaster Project { get; set; } = null!;

    [ForeignKey("StatusId")]
    public virtual StatusMaster Status { get; set; } = null!;

    [ForeignKey("ModuleId")]
    public virtual ModuleMaster? Module { get; set; }

    [InverseProperty("WorkEntry")]
    public virtual ICollection<WorkTimeLog> TimeLogs { get; set; } = new List<WorkTimeLog>();
}
