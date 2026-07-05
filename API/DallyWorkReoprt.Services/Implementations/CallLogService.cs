using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Services.Implementations;

public class CallLogService : ICallLogService
{
    private readonly ApplicationDbContext _context;

    public CallLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SyncCallLogsAsync(CallLogSyncRequestDto request, string currentUserName)
    {
        if (request == null || request.Logs == null || !request.Logs.Any())
            return 0;

        int addedCount = 0;
        var existingLogs = await _context.PhoneCallLogs
            .Where(x => x.EmployeeId == request.EmployeeId)
            .Select(x => new { x.PhoneNumber, x.StartTime, x.DurationInSeconds })
            .ToListAsync();

        foreach (var logDto in request.Logs)
        {
            // Simple deduplication check based on number, start time, and duration
            bool alreadyExists = existingLogs.Any(x =>
                x.PhoneNumber == logDto.PhoneNumber &&
                x.StartTime == logDto.StartTime &&
                x.DurationInSeconds == logDto.DurationInSeconds);

            if (!alreadyExists)
            {
                var phoneCallLog = new PhoneCallLog
                {
                    EmployeeId = request.EmployeeId,
                    PhoneNumber = logDto.PhoneNumber,
                    ContactName = logDto.ContactName,
                    CallType = logDto.CallType,
                    StartTime = logDto.StartTime,
                    EndTime = logDto.EndTime,
                    DurationInSeconds = logDto.DurationInSeconds,
                    SimId = logDto.SimId,
                    CreatedById = currentUserName ?? request.DeviceId
                };

                await _context.PhoneCallLogs.AddAsync(phoneCallLog);
                addedCount++;
            }
        }

        if (addedCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        return addedCount;
    }

    public async Task<System.Collections.Generic.IEnumerable<PhoneCallLogResponseDto>> GetAllAsync(int? employeeId)
    {
        var query = _context.PhoneCallLogs.Include(x => x.Employee).AsQueryable();

        if (employeeId.HasValue && employeeId.Value > 0)
        {
            query = query.Where(x => x.EmployeeId == employeeId.Value);
        }

        return await query.OrderByDescending(x => x.StartTime).Select(x => new PhoneCallLogResponseDto
        {
            CallLogId = x.CallLogId,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Employee.FirstName + " " + x.Employee.LastName,
            PhoneNumber = x.PhoneNumber,
            ContactName = x.ContactName,
            CallType = x.CallType,
            StartTime = x.StartTime,
            EndTime = x.EndTime,
            DurationInSeconds = x.DurationInSeconds,
            SimId = x.SimId,
            CreateDate = x.CreateDate
        }).ToListAsync();
    }
}
