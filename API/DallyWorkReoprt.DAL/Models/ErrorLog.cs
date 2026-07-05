using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("ErrorLog")]
public partial class ErrorLog
{
    [Key]
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? StackTrace { get; set; }

    [StringLength(200)]
    public string? Controller { get; set; }

    [StringLength(200)]
    public string? Action { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }
}
