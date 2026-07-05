using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers;

[Authorize]
public class RoleMasterSoftwareModulesController : BaseApiController
{
    private readonly IRoleMasterSoftwareModulesRepository _repository;

    public RoleMasterSoftwareModulesController(IRoleMasterSoftwareModulesRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("hierarchy/{roleId}")]
    public async Task<IActionResult> GetHierarchy(int roleId)
    {
        try
        {
            var hierarchy = await _repository.GetAccessControlHierarchyAsync(roleId);
            return Ok(ApiResponse<List<AccessControlDTO>>.SuccessResponse(hierarchy, "Hierarchy retrieved"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePermissions(RolePermissionSaveDTO model)
    {
        if (!ModelState.IsValid)
            return ValidationErrorResponse();

        try
        {
            var userId = User.Identity?.Name ?? "System";
            var result = await _repository.SaveRolePermissionsAsync(model, userId);

            if (result)
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Permissions saved successfully"));

            return BadRequest(ApiResponse<string>.ErrorResponse("Failed to save permissions"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}

