using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models;

public class MailTemplateDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TemplateName { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string SubjectFormat { get; set; } = null!;

    public string? HeaderHtml { get; set; }

    [Required]
    public string BodyHtml { get; set; } = null!;

    public string? FooterHtml { get; set; }

    public int CompanyId { get; set; }
    public int? EmployeeId { get; set; }

    public byte ActiveStatus { get; set; }
    public string? TableConfigJson { get; set; }
}
