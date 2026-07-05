namespace DallyWorkReoprt.DTO.Models;

public class ProjectDTO
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? ProjectColor { get; set; }
    public int CompanyId { get; set; }
    public byte ActiveStatus { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreatedById { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid? Guids { get; set; }
}
