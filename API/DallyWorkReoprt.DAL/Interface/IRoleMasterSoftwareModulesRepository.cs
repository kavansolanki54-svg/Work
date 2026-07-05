using DallyWorkReoprt.DTO.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IRoleMasterSoftwareModulesRepository
{
    Task<List<AccessControlDTO>> GetAccessControlHierarchyAsync(int roleId);
    Task<bool> SaveRolePermissionsAsync(RolePermissionSaveDTO model, string userId);
}

