using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

public partial class PhoneCallLog
{
    [Key]
    public int CallLogId { get; set; }

    public int EmployeeId { get; set; }

    [Required]
    public string PhoneNumber { get; set; } = null!;

    public string? ContactName { get; set; }

    [Required]
    public string CallType { get; set; } = null!; // Incoming, Outgoing, Missed, Rejected

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int DurationInSeconds { get; set; }

    public string? SimId { get; set; }

    public byte ActiveStatus { get; set; } = 1;

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(100)]
    public string CreatedById { get; set; } = null!;

    public Guid Guids { get; set; } = Guid.NewGuid();

    [ForeignKey("EmployeeId")]
    public virtual EmployeeMaster Employee { get; set; } = null!;

    public virtual ICollection<CallRecording> CallRecordings { get; set; } = new List<CallRecording>();
}
