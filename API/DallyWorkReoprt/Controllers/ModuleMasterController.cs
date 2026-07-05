using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ModuleMasterController : BaseApiController
{
    private readonly IModuleMasterRepository _moduleRepo;
    private readonly IMapper _mapper;

    public ModuleMasterController(IModuleMasterRepository moduleRepo, IMapper mapper)
    {
        _moduleRepo = moduleRepo;
        _mapper = mapper;
    }

    [HttpGet("List/{companyId}")]
    public IActionResult GetList(int companyId)
    {
        var modules = _moduleRepo.GetAll(companyId, activeStatus: 1).ToList();
        var dtos = _mapper.Map<List<ModuleDTO>>(modules);
        return Ok(ApiResponse<List<ModuleDTO>>.SuccessResponse(dtos, "Modules retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var module = await _moduleRepo.GetByIdAsync(id);
        if (module == null) 
            return NotFound(ApiResponse<ModuleDTO>.ErrorResponse("Module not found"));
        
        var dto = _mapper.Map<ModuleDTO>(module);
        return Ok(ApiResponse<ModuleDTO>.SuccessResponse(dto, "Module retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] ModuleDTO moduleDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var module = _mapper.Map<ModuleMaster>(moduleDTO);
        module.Guids = Guid.NewGuid();
        module.CreateDate = DateTime.Now;
        module.ActiveStatus = 1;
        
        var userName = User.Identity?.Name ?? "Admin";
        module.CreatedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;

        await _moduleRepo.AddAsync(module);
        await _moduleRepo.SaveAsync();

        return Ok(ApiResponse<ModuleMaster>.SuccessResponse(module, "Module saved successfully"));
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] ModuleDTO moduleDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var module = await _moduleRepo.GetByIdAsync(moduleDTO.ModuleId);
        if (module == null) 
            return NotFound(ApiResponse<ModuleMaster>.ErrorResponse("Module not found"));

        _mapper.Map(moduleDTO, module);
        
        var userName = User.Identity?.Name ?? "Admin";
        module.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
        module.ModifiedDate = DateTime.Now;

        await _moduleRepo.UpdateAsync(module);
        await _moduleRepo.SaveAsync();

        return Ok(ApiResponse<ModuleMaster>.SuccessResponse(module, "Module updated successfully"));
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var module = await _moduleRepo.GetByIdAsync(id);
        if (module == null) 
            return NotFound(ApiResponse<ModuleMaster>.ErrorResponse("Module not found"));

        module.ActiveStatus = 0; // Soft delete pattern
        
        var userName = User.Identity?.Name ?? "Admin";
        module.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
        module.ModifiedDate = DateTime.Now;

        await _moduleRepo.UpdateAsync(module);
        await _moduleRepo.SaveAsync();

        return Ok(ApiResponse<ModuleMaster>.SuccessResponse(module, "Module deleted successfully"));
    }
}
