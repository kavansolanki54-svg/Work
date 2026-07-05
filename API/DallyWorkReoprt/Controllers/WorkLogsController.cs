using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Services.Interfaces;
using DallyWorkReoprt.Models;
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

    [HttpGet("reset-schema-temp")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> ResetSchemaTemp([FromServices] DallyWorkReoprt.DAL.Models.ApplicationDbContext ctx)
    {
        var script = @"
IF OBJECT_ID('WorkLogTasks') IS NOT NULL DROP TABLE WorkLogTasks;
IF OBJECT_ID('WorkLogs') IS NOT NULL DROP TABLE WorkLogs;

CREATE TABLE [WorkLogs] (
    [WorkLogId] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [CompanyId] int NOT NULL,
    [ClientId] int NOT NULL,
    [ProjectId] int NOT NULL,
    [WorkDate] datetime2 NOT NULL,
    [InputTime] decimal(18,2) NOT NULL,
    [TotalDuration] decimal(18,2) NOT NULL,
    [TotalMinutes] int NOT NULL,
    [Mode] nvarchar(max) NOT NULL,
    [Remarks] nvarchar(max) NULL,
    [ActiveStatus] tinyint NOT NULL DEFAULT 1,
    [CreateDate] datetime2 NOT NULL DEFAULT getdate(),
    [CreatedById] nvarchar(100) NOT NULL,
    [ModifiedById] nvarchar(100) NULL,
    [ModifiedDate] datetime2 NULL,
    [Guids] uniqueidentifier NOT NULL DEFAULT newid(),
    [OtherEmployeeIds] nvarchar(max) NULL,
    [StatusId] int NOT NULL,
    CONSTRAINT [PK_WorkLogs] PRIMARY KEY ([WorkLogId])
);

CREATE TABLE [WorkLogTasks] (
    [WorkLogTaskId] int NOT NULL IDENTITY,
    [WorkLogId] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [StatusId] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    [CreateDate] datetime2 NOT NULL DEFAULT getdate(),
    [CreatedById] nvarchar(100) NOT NULL,
    [Guids] uniqueidentifier NOT NULL DEFAULT newid(),
    CONSTRAINT [PK_WorkLogTasks] PRIMARY KEY ([WorkLogTaskId])
);

ALTER TABLE [WorkLogs] ADD CONSTRAINT [FK_WorkLogs_ClientMaster_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [ClientMaster] ([ClientID]);
ALTER TABLE [WorkLogs] ADD CONSTRAINT [FK_WorkLogs_CompanyMaster_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CompanyMaster] ([CompanyID]);
ALTER TABLE [WorkLogs] ADD CONSTRAINT [FK_WorkLogs_EmployeeMaster_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [EmployeeMaster] ([EmployeeID]);
ALTER TABLE [WorkLogs] ADD CONSTRAINT [FK_WorkLogs_StatusMaster_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [StatusMaster] ([StatusID]);

ALTER TABLE [WorkLogTasks] ADD CONSTRAINT [FK_WorkLogTasks_StatusMaster_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [StatusMaster] ([StatusID]);
ALTER TABLE [WorkLogTasks] ADD CONSTRAINT [FK_WorkLogTasks_WorkLogs_WorkLogId] FOREIGN KEY ([WorkLogId]) REFERENCES [WorkLogs] ([WorkLogId]) ON DELETE CASCADE;
";
        await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(ctx.Database, script);
        return Ok("Tables reset successfully via raw sql.");
    }
}
