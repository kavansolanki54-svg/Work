namespace DallyWorkReoprt.DTO.Models;

public class AccessControlDTO
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
    public bool View { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public List<AccessControlDTO> Children { get; set; } = new();
}

public class RolePermissionSaveDTO
{
    public int RoleId { get; set; }
    public List<ModulePermissionUpdateDTO> Permissions { get; set; } = new();
}

public class ModulePermissionUpdateDTO
{
    public int ModuleId { get; set; }
    public bool View { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
}

