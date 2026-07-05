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
public class StatusMasterController : BaseApiController
{
    private readonly IStatusMasterRepository _statusRepo;
    private readonly IMapper _mapper;

    public StatusMasterController(IStatusMasterRepository statusRepo, IMapper mapper)
    {
        _statusRepo = statusRepo;
        _mapper = mapper;
    }

    [HttpGet("List/{companyId}")]
    public IActionResult GetList(int companyId)
    {
        var statuses = _statusRepo.GetAll(companyId, activeStatus: 1).ToList();
        var dtos = _mapper.Map<List<StatusDTO>>(statuses);
        return Ok(ApiResponse<List<StatusDTO>>.SuccessResponse(dtos, "Statuses retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var status = await _statusRepo.GetByIdAsync(id);
        if (status == null) 
            return NotFound(ApiResponse<StatusDTO>.ErrorResponse("Status not found"));
        
        var dto = _mapper.Map<StatusDTO>(status);
        return Ok(ApiResponse<StatusDTO>.SuccessResponse(dto, "Status retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] StatusDTO statusDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var status = _mapper.Map<StatusMaster>(statusDTO);
        status.Guids = Guid.NewGuid();
        status.CreateDate = DateTime.Now;
        status.ActiveStatus = 1;
        
        var userName = User.Identity?.Name ?? "Admin";
        status.CreatedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;

        await _statusRepo.AddAsync(status);
        await _statusRepo.SaveAsync();

        return Ok(ApiResponse<StatusMaster>.SuccessResponse(status, "Status saved successfully"));
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] StatusDTO statusDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var status = await _statusRepo.GetByIdAsync(statusDTO.StatusId);
        if (status == null) 
            return NotFound(ApiResponse<StatusMaster>.ErrorResponse("Status not found"));

        _mapper.Map(statusDTO, status);
        
        var userName = User.Identity?.Name ?? "Admin";
        status.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
        status.ModifiedDate = DateTime.Now;

        await _statusRepo.UpdateAsync(status);
        await _statusRepo.SaveAsync();

        return Ok(ApiResponse<StatusMaster>.SuccessResponse(status, "Status updated successfully"));
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var status = await _statusRepo.GetByIdAsync(id);
        if (status == null) 
            return NotFound(ApiResponse<StatusMaster>.ErrorResponse("Status not found"));

        status.ActiveStatus = 0; // Soft delete pattern
        
        var userName = User.Identity?.Name ?? "Admin";
        status.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
        status.ModifiedDate = DateTime.Now;

        await _statusRepo.UpdateAsync(status);
        await _statusRepo.SaveAsync();

        return Ok(ApiResponse<StatusMaster>.SuccessResponse(status, "Status deleted successfully"));
    }
}
