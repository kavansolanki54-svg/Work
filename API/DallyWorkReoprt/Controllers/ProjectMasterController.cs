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
public class ProjectMasterController : BaseApiController
{
    private readonly IProjectMasterRepository _projectRepo;
    private readonly IMapper _mapper;

    public ProjectMasterController(IProjectMasterRepository projectRepo, IMapper mapper)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
    }

    [HttpGet("List/{companyId}")]
    public IActionResult GetList(int companyId)
    {
        var projects = _projectRepo.GetAll(companyId, activeStatus: 1).ToList();
        var dtos = _mapper.Map<List<ProjectDTO>>(projects);
        return Ok(ApiResponse<List<ProjectDTO>>.SuccessResponse(dtos, "Projects retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project == null)
            return NotFound(ApiResponse<ProjectDTO>.ErrorResponse("Project not found"));

        var dto = _mapper.Map<ProjectDTO>(project);
        return Ok(ApiResponse<ProjectDTO>.SuccessResponse(dto, "Project retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] ProjectDTO projectDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        try
        {
            var project = _mapper.Map<ProjectMaster>(projectDTO);
            project.Guids = Guid.NewGuid();
            project.CreateDate = DateTime.Now;
            project.ActiveStatus = 1;

            var userName = User.Identity?.Name ?? "Admin";
            project.CreatedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;

            await _projectRepo.AddAsync(project);

            return Ok(ApiResponse<ProjectMaster>.SuccessResponse(project, "Project saved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] ProjectDTO projectDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        try
        {
            var project = await _projectRepo.GetByIdAsync(projectDTO.ProjectId);
            if (project == null)
                return NotFound(ApiResponse<ProjectMaster>.ErrorResponse("Project not found"));

            _mapper.Map(projectDTO, project);

            var userName = User.Identity?.Name ?? "Admin";
            project.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
            project.ModifiedDate = DateTime.Now;

            await _projectRepo.UpdateAsync(project);

            return Ok(ApiResponse<ProjectMaster>.SuccessResponse(project, "Project updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null)
                return NotFound(ApiResponse<ProjectMaster>.ErrorResponse("Project not found"));

            project.ActiveStatus = 0; // Soft delete pattern

            var userName = User.Identity?.Name ?? "Admin";
            project.ModifiedById = userName.Length > 20 ? userName.Substring(0, 20) : userName;
            project.ModifiedDate = DateTime.Now;

            await _projectRepo.UpdateAsync(project);

            return Ok(ApiResponse<ProjectMaster>.SuccessResponse(project, "Project deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}
