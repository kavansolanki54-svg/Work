using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class SoftwareModulesRepository : ISoftwareModulesRepository
{
    private readonly ApplicationDbContext _context;

    public SoftwareModulesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuDTO>> GetMenuByRoleAsync(int roleId, bool isTenant)
    {
        List<MenuDTO> modules;

        if (isTenant)
        {
            modules = await _context.SoftwareModules
                .Where(m => m.ActiveStatus == 1)
                .OrderBy(m => m.DisplayOrder ?? 9999)
                .ThenBy(m => m.ModulesName)
                .Select(m => new MenuDTO
                {
                    ModuleId = m.SoftwareModulesMasterId,
                    Name = m.ModulesName,
                    Controller = m.ControllersName,
                    Action = m.ActionName,
                    Icon = m.Icon,
                    Url = !string.IsNullOrWhiteSpace(m.FullUrl) ? m.FullUrl : (!string.IsNullOrWhiteSpace(m.ControllersName) ? $"/{m.ControllersName.ToLower().Replace("controller", "").Replace("master", "")}" : $"/module-{m.SoftwareModulesMasterId}"),
                    ParentId = m.ParentId,
                    CanCreate = true,
                    CanEdit = true,
                    CanDelete = true,
                    DisplayOrder = m.DisplayOrder ?? 9999
                })
                .ToListAsync();
        }
        else
        {
            var rolePermissions = await _context.RoleSoftwareModules
                .Where(r => r.RoleMasterId == roleId && r.ActiveStatus == 1)
                .ToListAsync();

            var visibleModuleIds = rolePermissions
                .Where(r => r.View == true)
                .Select(r => r.SoftwareModulesMasterId)
                .ToList();

            if (!visibleModuleIds.Any())
                return new List<MenuDTO>();

            var allActiveModules = await _context.SoftwareModules
                .Where(m => m.ActiveStatus == 1)
                .ToListAsync();

            var includedIds = new HashSet<int>();
            foreach (var moduleId in visibleModuleIds)
            {
                var currentId = (int?)moduleId;
                while (currentId.HasValue && !includedIds.Contains(currentId.Value))
                {
                    includedIds.Add(currentId.Value);
                    currentId = allActiveModules.FirstOrDefault(m => m.SoftwareModulesMasterId == currentId.Value)?.ParentId;
                }
            }

            modules = allActiveModules
                .Where(m => includedIds.Contains(m.SoftwareModulesMasterId))
                .Select(m =>
                {
                    var perm = rolePermissions.FirstOrDefault(r => r.SoftwareModulesMasterId == m.SoftwareModulesMasterId);
                    return new MenuDTO
                    {
                        ModuleId = m.SoftwareModulesMasterId,
                        Name = m.ModulesName,
                        Controller = m.ControllersName,
                        Action = m.ActionName,
                        Icon = m.Icon,
                        Url = !string.IsNullOrWhiteSpace(m.FullUrl) ? m.FullUrl : (!string.IsNullOrWhiteSpace(m.ControllersName) ? $"/{m.ControllersName.ToLower().Replace("controller", "").Replace("master", "")}" : $"/module-{m.SoftwareModulesMasterId}"),
                        ParentId = m.ParentId,
                        CanCreate = perm?.Add ?? false,
                        CanEdit = perm?.Edit ?? false,
                        CanDelete = perm?.Delete ?? false,
                        DisplayOrder = m.DisplayOrder ?? 9999
                    };
                })
                .ToList();
        }

        return BuildMenuHierarchy(modules, null);
    }

    private List<MenuDTO> BuildMenuHierarchy(List<MenuDTO> modules, int? parentId)
    {
        var result = new List<MenuDTO>();
        var items = modules.Where(m => m.ParentId == parentId)
            .OrderBy(m => m.DisplayOrder)
            .ThenBy(m => m.Name)
            .ToList();

        foreach (var item in items)
        {
            var menu = new MenuDTO
            {
                ModuleId = item.ModuleId,
                Name = item.Name,
                Controller = item.Controller,
                Action = item.Action,
                Icon = item.Icon,
                Url = item.Url,
                ParentId = item.ParentId,
                CanCreate = item.CanCreate,
                CanEdit = item.CanEdit,
                CanDelete = item.CanDelete,
                DisplayOrder = item.DisplayOrder,
                SubMenus = BuildMenuHierarchy(modules, item.ModuleId)
            };
            result.Add(menu);
        }

        return result;
    }
}

