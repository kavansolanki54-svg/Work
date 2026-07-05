using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using DallyWorkReoprt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReportsController : BaseApiController
{
    private readonly IReportService _service;
    private readonly IEmailService _emailService;

    public ReportsController(IReportService service, IEmailService emailService)
    {
        _service = service;
        _emailService = emailService;
    }

    [HttpGet("List/{employeeId}")]
    public async Task<IActionResult> GetReports(int employeeId)
    {
        var uid = employeeId > 0 ? employeeId : CurrentEmployeeId;
        var reports = await _service.GetAllAsync(uid);
        return Ok(ApiResponse<IEnumerable<ReportResponseDto>>.SuccessResponse(reports, "Reports retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReport(int id)
    {
        var report = await _service.GetByIdAsync(id);
        if (report == null)
            return NotFound(ApiResponse<ReportResponseDto>.ErrorResponse("Report not found"));

        return Ok(ApiResponse<ReportResponseDto>.SuccessResponse(report, "Report retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] ReportCreateDto reportDto)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var created = await _service.CreateAsync(reportDto, CurrentEmployeeId, CurrentUserName);
        return Ok(ApiResponse<ReportResponseDto>.SuccessResponse(created, "Report saved successfully"));
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportUpdateDto reportDto)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var success = await _service.UpdateAsync(id, reportDto, CurrentUserName);

        if (!success)
            return NotFound(ApiResponse<bool>.ErrorResponse("Report not found or update failed"));

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Report updated successfully"));
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id, CurrentUserName);

        if (!success)
            return NotFound(ApiResponse<bool>.ErrorResponse("Report not found"));

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Report deleted successfully"));
    }

    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendEmail(int id)
    {
        try
        {
            await _emailService.SendDailyReportEmailAsync(id);
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
            var html = await _emailService.GetDailyTaskSheetPreviewAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(html, "Preview generated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}
