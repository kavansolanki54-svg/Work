using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("WorkLogs")]
public partial class WorkLog
{
    [Key]
    public int WorkLogId { get; set; }

    public int EmployeeId { get; set; }

    public int CompanyId { get; set; }

    public int ClientId { get; set; }

    public int ProjectId { get; set; }

    public DateTime WorkDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal InputTime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalDuration { get; set; }

    public int TotalMinutes { get; set; }

    [StringLength(50)]
    public string Mode { get; set; } = null!;

    public string? Remarks { get; set; }

    public byte ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    public string CreatedById { get; set; } = null!;

    [Column("ModifiedBYID")]
    [StringLength(100)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }
    public string? OtherEmployeeIds { get; set; }

    public int StatusId { get; set; }
    public bool IsEmailSent { get; set; }
    public DateTime? EmailSentDate { get; set; }

    [ForeignKey("ClientId")]
    public virtual ClientMaster Client { get; set; } = null!;

    [ForeignKey("ProjectId")]
    public virtual ProjectMaster Project { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual EmployeeMaster Employee { get; set; } = null!;

    [ForeignKey("StatusId")]
    public virtual StatusMaster Status { get; set; } = null!;

    [InverseProperty("WorkLog")]
    public virtual ICollection<WorkLogTask> WorkLogTasks { get; set; } = new List<WorkLogTask>();
}
