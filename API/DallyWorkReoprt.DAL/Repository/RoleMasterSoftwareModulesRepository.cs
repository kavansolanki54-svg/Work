using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class RoleMasterSoftwareModulesRepository : IRoleMasterSoftwareModulesRepository
{
    private readonly ApplicationDbContext _context;

    public RoleMasterSoftwareModulesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccessControlDTO>> GetAccessControlHierarchyAsync(int roleId)
    {
        // 1. Fetch all active modules
        var allModules = await _context.SoftwareModules
            .Where(m => m.ActiveStatus == 1)
            .OrderBy(m => m.DisplayOrder ?? 9999)
            .ToListAsync();

        // 2. Fetch existing permissions for this role
        var permissions = await _context.RoleSoftwareModules
            .Where(r => r.RoleMasterId == roleId && r.ActiveStatus == 1)
            .ToDictionaryAsync(r => r.SoftwareModulesMasterId);

        // 3. Build hierarchy
        return BuildHierarchy(allModules, permissions, null);
    }

    private List<AccessControlDTO> BuildHierarchy(
        List<SoftwareModulesMaster> modules,
        Dictionary<int, RoleMasterSoftwareModule> permissions,
        int? parentId)
    {
        var result = new List<AccessControlDTO>();
        var items = modules.Where(m => m.ParentId == parentId).ToList();

        foreach (var item in items)
        {
            var dto = new AccessControlDTO
            {
                ModuleId = item.SoftwareModulesMasterId,
                Name = item.ModulesName,
                ParentId = item.ParentId,
                Children = BuildHierarchy(modules, permissions, item.SoftwareModulesMasterId)
            };

            if (permissions.TryGetValue(item.SoftwareModulesMasterId, out var p))
            {
                dto.View = p.View ?? false;
                dto.Add = p.Add ?? false;
                dto.Edit = p.Edit ?? false;
                dto.Delete = p.Delete ?? false;
            }
            else
            {
                dto.View = false;
                dto.Add = false;
                dto.Edit = false;
                dto.Delete = false;
            }

            result.Add(dto);
        }

        return result;
    }

    public async Task<bool> SaveRolePermissionsAsync(RolePermissionSaveDTO model, string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Get existing permissions
            var existing = await _context.RoleSoftwareModules
                .Where(r => r.RoleMasterId == model.RoleId)
                .ToListAsync();

            // 2. Map existing by module ID
            var existingMap = existing.ToDictionary(r => r.SoftwareModulesMasterId);

            foreach (var p in model.Permissions)
            {
                if (existingMap.TryGetValue(p.ModuleId, out var record))
                {
                    // Update
                    record.View = p.View;
                    record.Add = p.Add;
                    record.Edit = p.Edit;
                    record.Delete = p.Delete;
                    record.ModifiedById = userId;
                    record.ModifiedDate = DateTime.Now;
                    record.ActiveStatus = 1;
                }
                else
                {
                    // Insert
                    var newRecord = new RoleMasterSoftwareModule
                    {
                        RoleMasterId = model.RoleId,
                        SoftwareModulesMasterId = p.ModuleId,
                        View = p.View,
                        Add = p.Add,
                        Edit = p.Edit,
                        Delete = p.Delete,
                        CreatedById = userId,
                        CreateDate = DateTime.Now,
                        ActiveStatus = 1,
                        Guids = Guid.NewGuid()
                    };
                    _context.RoleSoftwareModules.Add(newRecord);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

