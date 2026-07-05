using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("WorkLogTasks")]
public partial class WorkLogTask
{
    [Key]
    public int WorkLogTaskId { get; set; }

    public int WorkLogId { get; set; }

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }

    public bool IsCompleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    public string CreatedById { get; set; } = null!;

    public Guid Guids { get; set; }

    [ForeignKey("WorkLogId")]
    public virtual WorkLog WorkLog { get; set; } = null!;

    [ForeignKey("StatusId")]
    public virtual StatusMaster Status { get; set; } = null!;
}
