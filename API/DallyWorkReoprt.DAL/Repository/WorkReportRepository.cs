using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class WorkReportRepository : GenericRepository<WorkReport>, IWorkReportRepository
{
    public WorkReportRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WorkReport>> GetFullReportsAsync(int employeeId)
    {
        return await _context.WorkReports
            .Where(x => x.EmployeeId == employeeId && x.ActiveStatus == 1)
            .Include(x => x.WorkEntries)
                .ThenInclude(w => w.TimeLogs)
            .Include(x => x.WorkEntries)
                .ThenInclude(w => w.Project)
            .Include(x => x.WorkEntries)
                .ThenInclude(w => w.Status)
            .Include(x => x.Employee)
            .OrderByDescending(x => x.ReportDate)
            .ToListAsync();
    }

    public async Task<WorkReport?> GetFullReportByIdAsync(int id)
    {
        return await _context.WorkReports
            .Include(x => x.WorkEntries)
                .ThenInclude(w => w.TimeLogs)
            .Include(x => x.WorkEntries)
                .ThenInclude(w => w.Project)
            .Include(x => x.WorkEntries)
                .ThenInclude(w => w.Status)
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
