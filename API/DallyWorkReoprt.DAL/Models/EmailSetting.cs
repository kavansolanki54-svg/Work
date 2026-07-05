using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

public partial class EmailSetting
{
    [Key]
    public int EmailSettingsId { get; set; }

    [StringLength(255)]
    public string SmtpServer { get; set; } = null!;

    public int Port { get; set; }

    [StringLength(255)]
    public string SenderName { get; set; } = null!;

    [StringLength(255)]
    public string SenderEmail { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    public bool ActiveStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CreateDate { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmailSettings")]
    public virtual EmployeeMaster? Employee { get; set; }
}
