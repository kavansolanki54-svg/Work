using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

public partial class EmailRecipient
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(10)]
    public string RecipientType { get; set; } = null!;

    public bool ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmailRecipients")]
    public virtual EmployeeMaster? Employee { get; set; }
}
