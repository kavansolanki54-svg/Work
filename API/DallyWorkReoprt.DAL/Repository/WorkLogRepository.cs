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

        return await query.OrderByDescending(x => x.WorkDate).ToListAsync();
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
        var logs = await _context.WorkLogs
            .Include(x => x.WorkLogTasks)
            .Where(x => x.EmployeeId == employeeId && x.WorkDate.Date == date.Date)
            .ToListAsync();

        if (logs.Any())
        {
            foreach (var log in logs)
            {
                _context.WorkLogTasks.RemoveRange(log.WorkLogTasks);
            }
            _context.WorkLogs.RemoveRange(logs);
            await _context.SaveChangesAsync();
        }
    }
}
