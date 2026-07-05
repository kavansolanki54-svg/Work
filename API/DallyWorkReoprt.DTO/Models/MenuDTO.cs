namespace DallyWorkReoprt.DTO.Models;

public class MenuDTO
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = null!;
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string? Icon { get; set; }
    public string? Url { get; set; }
    public int? ParentId { get; set; }
    public bool? CanCreate { get; set; }
    public bool? CanEdit { get; set; }
    public bool? CanDelete { get; set; }
    public int DisplayOrder { get; set; }
    public List<MenuDTO> SubMenus { get; set; } = new List<MenuDTO>();
}

