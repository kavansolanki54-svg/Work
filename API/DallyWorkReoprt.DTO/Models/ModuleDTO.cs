namespace DallyWorkReoprt.DTO.Models;

public class ModuleDTO
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = null!;
    public int? ParentModuleId { get; set; }
    public int CompanyId { get; set; }
    public byte ActiveStatus { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreatedById { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid? Guids { get; set; }
}
