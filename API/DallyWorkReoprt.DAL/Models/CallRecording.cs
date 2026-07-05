using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

public partial class CallRecording
{
    [Key]
    public int RecordingId { get; set; }

    public int CallLogId { get; set; }

    [Required]
    public string RecordingUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CallLogId")]
    public virtual PhoneCallLog PhoneCallLog { get; set; } = null!;
}
