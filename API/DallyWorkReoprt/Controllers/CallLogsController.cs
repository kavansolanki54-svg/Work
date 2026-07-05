using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Services.Interfaces;
using DallyWorkReoprt.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CallLogsController : BaseApiController
{
    private readonly ICallLogService _service;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
    private readonly DallyWorkReoprt.DAL.Models.ApplicationDbContext _context;

    public CallLogsController(ICallLogService service, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, DallyWorkReoprt.DAL.Models.ApplicationDbContext context)
    {
        _service = service;
        _env = env;
        _context = context;
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

    [HttpPost("UploadRecording/{callLogId}")]
    public async Task<IActionResult> UploadRecording(int callLogId, Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded."));

        try
        {
            var callLog = await _context.PhoneCallLogs.FindAsync(callLogId);
            if (callLog == null) return NotFound(ApiResponse<object>.ErrorResponse("Call log not found."));

            var recordingUrl = await SaveFileAndGetUrl(file);
            await SaveRecordingToDb(callLogId, recordingUrl);

            return Ok(ApiResponse<string>.SuccessResponse(recordingUrl, "Recording uploaded successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("UploadNativeRecording")]
    public async Task<IActionResult> UploadNativeRecording([FromQuery] int employeeId, [FromQuery] string phoneNumber, [FromQuery] DateTime startTime, Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded."));

        try
        {
            // Find the exact call log
            // Since DateTime from Android might have slight millisecond differences, we allow a small window of 5 seconds
            var minTime = startTime.AddSeconds(-5);
            var maxTime = startTime.AddSeconds(5);
            
            var callLog = await _context.PhoneCallLogs
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId 
                                       && x.PhoneNumber == phoneNumber 
                                       && x.StartTime >= minTime 
                                       && x.StartTime <= maxTime);

            if (callLog == null) return NotFound(ApiResponse<object>.ErrorResponse("Call log not found on server for the given details."));

            var recordingUrl = await SaveFileAndGetUrl(file);
            await SaveRecordingToDb(callLog.CallLogId, recordingUrl);

            return Ok(ApiResponse<string>.SuccessResponse(recordingUrl, "Native Recording uploaded successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    private async Task<string> SaveFileAndGetUrl(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var uploadsFolder = System.IO.Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "recordings");
        if (!System.IO.Directory.Exists(uploadsFolder)) System.IO.Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = System.IO.Path.Combine(uploadsFolder, fileName);

        using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/recordings/{fileName}";
    }

    private async Task SaveRecordingToDb(int callLogId, string recordingUrl)
    {
        var recording = new DallyWorkReoprt.DAL.Models.CallRecording
        {
            CallLogId = callLogId,
            RecordingUrl = recordingUrl
        };

        await _context.CallRecordings.AddAsync(recording);
        await _context.SaveChangesAsync();
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
