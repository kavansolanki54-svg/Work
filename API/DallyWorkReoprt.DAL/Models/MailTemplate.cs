using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models;

[Table("MailTemplates")]
public partial class MailTemplate
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string TemplateName { get; set; } = null!;

    [StringLength(200)]
    public string SubjectFormat { get; set; } = null!;

    public string? HeaderHtml { get; set; }

    public string BodyHtml { get; set; } = null!;

    public string? FooterHtml { get; set; }

    public int CompanyId { get; set; }
    public int? EmployeeId { get; set; }

    public byte ActiveStatus { get; set; }

    [Column("CreatedByID")]
    [StringLength(20)]
    public string? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column("ModifiedBYID")]
    [StringLength(20)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public string? TableConfigJson { get; set; }

    public Guid Guids { get; set; }
}
