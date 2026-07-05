using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class WorkLogRepository : GenericRepository<WorkLog>, IWorkLogRepository
{
    public WorkLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WorkLog>> GetLogsAsync(int companyId, int? employeeId = null)
    {
        var query = _context.WorkLogs
            .Include(x => x.Employee) // Added to fix NullRef in email dispatch
            .Include(x => x.Client)
            .Include(x => x.Project)
            .Include(x => x.WorkLogTasks)
                .ThenInclude(t => t.Status)
            .Where(x => x.CompanyId == companyId && x.ActiveStatus == 1);

        if (employeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == employeeId.Value);
        }

        return await query.OrderByDescending(x => x.WorkDate).ThenBy(x => x.WorkLogId).ToListAsync();
    }

    public async Task<IEnumerable<WorkLog>> GetLogsByDateAsync(int employeeId, DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        return await _context.WorkLogs
            .Include(x => x.Employee)
            .Include(x => x.Client)
            .Include(x => x.Project)
            .Include(x => x.WorkLogTasks)
                .ThenInclude(t => t.Status)
            .Where(x => x.EmployeeId == employeeId && x.WorkDate >= startDate && x.WorkDate < endDate && x.ActiveStatus == 1)
            .OrderBy(x => x.WorkLogId)
            .ToListAsync();
    }

    public async Task<WorkLog?> GetLogByIdAsync(int id)
    {
        return await _context.WorkLogs
            .Include(x => x.Employee)
            .Include(x => x.Client)
            .Include(x => x.Project)
            .Include(x => x.WorkLogTasks)
                .ThenInclude(t => t.Status)
            .FirstOrDefaultAsync(x => x.WorkLogId == id);
    }

    public async Task DeleteByDateAsync(int employeeId, DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        var logIds = _context.WorkLogs
            .Where(x => x.EmployeeId == employeeId && x.WorkDate >= startDate && x.WorkDate < endDate)
            .Select(x => x.WorkLogId);

        await _context.WorkLogTasks
            .Where(t => logIds.Contains(t.WorkLogId))
            .ExecuteDeleteAsync();

        await _context.WorkLogs
            .Where(x => x.EmployeeId == employeeId && x.WorkDate >= startDate && x.WorkDate < endDate)
            .ExecuteDeleteAsync();
    }
}
