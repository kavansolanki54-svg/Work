using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using DallyWorkReoprt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkLogsController : BaseApiController
{
    private readonly IWorkLogService _service;
    private readonly IEmailService _emailService;

    public WorkLogsController(IWorkLogService service, IEmailService emailService)
    {
        _service = service;
        _emailService = emailService;
    }

    [HttpGet("List/{employeeId?}")]
    public async Task<IActionResult> GetAll(int? employeeId)
    {
        var logs = await _service.GetAllAsync(CurrentCompanyId, employeeId);
        return Ok(ApiResponse<IEnumerable<WorkLogResponseDto>>.SuccessResponse(logs, "Logs retrieved successfully"));
    }
    
    [HttpGet("ByDate/{date}/{employeeId?}")]
    public async Task<IActionResult> GetByDate(DateTime date, int? employeeId)
    {
        var logs = await _service.GetByDateAsync(date, CurrentCompanyId, employeeId ?? CurrentEmployeeId);
        return Ok(ApiResponse<IEnumerable<WorkLogResponseDto>>.SuccessResponse(logs, "Logs retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var log = await _service.GetByIdAsync(id);
        if (log == null) return NotFound(ApiResponse<object>.ErrorResponse("Work log not found"));
        return Ok(ApiResponse<WorkLogResponseDto>.SuccessResponse(log, "Log retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Create(WorkLogCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        try
        {
            var log = await _service.CreateAsync(dto, CurrentEmployeeId, CurrentCompanyId, CurrentUserName);
            return Ok(ApiResponse<WorkLogResponseDto>.SuccessResponse(log, "Work log saved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("SaveSession")]
    public async Task<IActionResult> SaveSession(WorkReportSessionDto session)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();
        try
        {
            await _service.SaveSessionAsync(session, CurrentEmployeeId, CurrentCompanyId, CurrentUserName);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Daily report session saved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, WorkLogCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        try
        {
            var success = await _service.UpdateAsync(id, dto, CurrentUserName);
            if (!success) return NotFound(ApiResponse<object>.ErrorResponse("Work log not found"));
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Work log updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id, CurrentUserName);
        if (!success) return NotFound(ApiResponse<object>.ErrorResponse("Work log not found"));
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Work log deleted successfully"));
    }

    [HttpDelete("DeleteSession/{date}")]
    public async Task<IActionResult> DeleteSession(DateTime date)
    {
        try
        {
            await _service.DeleteSessionByDateAsync(date, CurrentEmployeeId, CurrentUserName);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Daily session deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendEmail(int id)
    {
        try
        {
            await _emailService.SendWorkLogEmailAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Email sent successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("{id}/preview-email")]
    public async Task<IActionResult> PreviewEmail(int id)
    {
        try
        {
            var html = await _emailService.GetDailyWorkReportPreviewAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(html, "Preview generated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}
