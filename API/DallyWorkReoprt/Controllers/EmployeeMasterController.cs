using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using DallyWorkReoprt.Utilities.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmployeeMasterController : BaseApiController
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public EmployeeMasterController(IEmployeeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("list/{companyId}")]
    public async Task<IActionResult> GetEmployees(int companyId)
    {
        try
        {
            var employees = await _repository.GetAll(companyId).ToListAsync();
            var dtos = _mapper.Map<List<EmployeeMasterDTO>>(employees);
            return Ok(ApiResponse<List<EmployeeMasterDTO>>.SuccessResponse(dtos, "Employees retrieved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Employee not found"));

            var dto = _mapper.Map<EmployeeMasterDTO>(employee);

            // Decrypt password so it can be viewed/edited in the frontend as requested
            try
            {
                if (!string.IsNullOrEmpty(employee.Passwords))
                {
                    dto.Passwords = new EncryptionHelper().Decrypt(employee.Passwords);
                }
            }
            catch
            {
                dto.Passwords = "";
            }

            return Ok(ApiResponse<EmployeeMasterDTO>.SuccessResponse(dto, "Employee retrieved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveEmployee(EmployeeMasterDTO model)
    {
        if (string.IsNullOrWhiteSpace(model.Passwords))
        {
            ModelState.AddModelError("Passwords", "Password is required for new employees.");
        }

        if (!ModelState.IsValid)
            return ValidationErrorResponse();

        try
        {
            if (_repository.EmployeeCodeExists(model.CompanyId, model.EmployeeCode, null))
            {
                ModelState.AddModelError("EmployeeCode", "Employee Code already exists.");
                return ValidationErrorResponse();
            }

            var employee = _mapper.Map<EmployeeMaster>(model);
            employee.Passwords = new EncryptionHelper().Encrypt(model.Passwords);
            employee.ActiveStatus = 1;
            employee.CreateDate = DateTime.Now;
            employee.CreatedById = User.Identity?.Name ?? "System";
            employee.Guids = Guid.NewGuid();
            employee.EmployeeName = $"{model.FirstName} {model.MiddleName} {model.LastName}".Trim();
            employee.IsAllowLogin = (byte)(model.IsAllowLogin ? 1 : 0);

            await _repository.AddAsync(employee);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Employee created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateEmployee(EmployeeMasterDTO model)
    {
        if (!ModelState.IsValid)
            return ValidationErrorResponse();

        try
        {
            var existing = await _repository.GetByIdAsync(model.EmployeeId);
            if (existing == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Employee not found"));

            if (_repository.EmployeeCodeExists(model.CompanyId, model.EmployeeCode, model.EmployeeId))
            {
                ModelState.AddModelError("EmployeeCode", "Employee Code already exists.");
                return ValidationErrorResponse();
            }

            // Map basic fields
            existing.FirstName = model.FirstName;
            existing.MiddleName = model.MiddleName;
            existing.LastName = model.LastName;
            existing.Designation = model.Designation;
            existing.Email = model.Email;
            existing.MobileNo = model.MobileNo;
            existing.IsAllowLogin = (byte)(model.IsAllowLogin ? 1 : 0);
            existing.EmployeeCode = model.EmployeeCode;
            existing.GenderId = model.GenderId;
            existing.RoleMasterId = model.RoleMasterId;
            existing.EmployeeName = $"{model.FirstName} {model.MiddleName} {model.LastName}".Trim();

            // Only update password if provided
            if (!string.IsNullOrWhiteSpace(model.Passwords))
            {
                existing.Passwords = new EncryptionHelper().Encrypt(model.Passwords);
            }

            existing.ModifiedById = User.Identity?.Name ?? "System";
            existing.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(existing);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Employee updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Employee not found"));

            existing.ActiveStatus = 0;
            existing.ModifiedById = User.Identity?.Name ?? "System";
            existing.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(existing);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Employee deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("update-break-duration/{employeeId}/{duration}")]
    public async Task<IActionResult> UpdateBreakDuration(int employeeId, int duration)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(employeeId);
            if (existing == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Employee not found"));

            existing.DefaultBreakDuration = duration;
            existing.ModifiedById = User.Identity?.Name ?? "System";
            existing.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(existing);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Break duration updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}

