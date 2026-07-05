namespace DallyWorkReoprt.DTO.Models;

public class RoleDTO
{
    public int RoleMasterId { get; set; }
    public int CompanyId { get; set; }
    public string RoleName { get; set; } = null!;
    public int RoleTypeId { get; set; }
    public string? Descriptions { get; set; }
    public byte ActiveStatus { get; set; }
}

