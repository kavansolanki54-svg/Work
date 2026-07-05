using DallyWorkReoprt.DTO.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface ISoftwareModulesRepository
{
    Task<List<MenuDTO>> GetMenuByRoleAsync(int roleId, bool isTenant);
}

