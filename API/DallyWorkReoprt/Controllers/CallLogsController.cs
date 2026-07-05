using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Services.Interfaces;
using DallyWorkReoprt.Models;

namespace DallyWorkReoprt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CallLogsController : BaseApiController
{
    private readonly ICallLogService _service;

    public CallLogsController(ICallLogService service)
    {
        _service = service;
    }

    [HttpGet("List/{employeeId?}")]
    public async Task<IActionResult> GetAll(int? employeeId)
    {
        var logs = await _service.GetAllAsync(employeeId);
        return Ok(ApiResponse<System.Collections.Generic.IEnumerable<PhoneCallLogResponseDto>>.SuccessResponse(logs, "Call logs retrieved successfully"));
    }

    [HttpPost("Sync")]
    public async Task<IActionResult> Sync(CallLogSyncRequestDto dto)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        try
        {
            // We use CurrentUserName to track who created the log (usually email or name based on auth token)
            // The dto.EmployeeId is sent explicitly from the Android app
            int syncedCount = await _service.SyncCallLogsAsync(dto, CurrentUserName);
            
            return Ok(ApiResponse<int>.SuccessResponse(syncedCount, $"Successfully synced {syncedCount} call logs."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("reset-schema-temp")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> ResetSchemaTemp([FromServices] DallyWorkReoprt.DAL.Models.ApplicationDbContext ctx)
    {
        var script = @"
IF OBJECT_ID('PhoneCallLogs') IS NOT NULL DROP TABLE PhoneCallLogs;

CREATE TABLE [PhoneCallLogs] (
    [CallLogId] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [ContactName] nvarchar(max) NULL,
    [CallType] nvarchar(max) NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NULL,
    [DurationInSeconds] int NOT NULL,
    [SimId] nvarchar(max) NULL,
    [ActiveStatus] tinyint NOT NULL DEFAULT 1,
    [CreateDate] datetime2 NOT NULL DEFAULT getdate(),
    [CreatedById] nvarchar(100) NOT NULL,
    [Guids] uniqueidentifier NOT NULL DEFAULT newid(),
    CONSTRAINT [PK_PhoneCallLogs] PRIMARY KEY ([CallLogId])
);

ALTER TABLE [PhoneCallLogs] ADD CONSTRAINT [FK_PhoneCallLogs_EmployeeMaster_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [EmployeeMaster] ([EmployeeID]);
";
        await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(ctx.Database, script);
        return Ok("PhoneCallLogs table created successfully via raw sql.");
    }
}
