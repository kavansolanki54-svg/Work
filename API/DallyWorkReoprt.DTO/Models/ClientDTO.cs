namespace DallyWorkReoprt.DTO.Models;

public class ClientDTO
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = null!;
    public string? ClientShortCode { get; set; }
    public int CompanyId { get; set; }
    public byte ActiveStatus { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? CreatedById { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid? Guids { get; set; }
}
