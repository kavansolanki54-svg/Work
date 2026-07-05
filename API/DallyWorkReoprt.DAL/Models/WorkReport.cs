using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("WorkReports")]
public partial class WorkReport
{
    [Key]
    public int Id { get; set; }

    public DateOnly ReportDate { get; set; }

    public int EmployeeId { get; set; }

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

    [ForeignKey("EmployeeId")]
    public virtual EmployeeMaster Employee { get; set; } = null!;

    [InverseProperty("WorkReport")]
    public virtual ICollection<WorkEntry> WorkEntries { get; set; } = new List<WorkEntry>();
}
