namespace DallyWorkReoprt.DTO.Models;

public class StatusDTO
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public string? StatusColor { get; set; }
    public int CompanyId { get; set; }
    public byte ActiveStatus { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreatedById { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid? Guids { get; set; }
}
