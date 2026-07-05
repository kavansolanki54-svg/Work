using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("WorkTimeLogs")]
public partial class WorkTimeLog
{
    [Key]
    public int Id { get; set; }

    public int WorkEntryId { get; set; }

    [StringLength(10)]
    public string InTime { get; set; } = null!;

    [StringLength(10)]
    public string OutTime { get; set; } = null!;

    public bool Is30MinBreak { get; set; }

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

    [ForeignKey("WorkEntryId")]
    public virtual WorkEntry WorkEntry { get; set; } = null!;
}
