using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Stats")]
        public async Task<IActionResult> GetStats([FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                int companyId = CurrentCompanyId;
                int employeeId = CurrentEmployeeId;

                var roleClaim = User.FindFirst("RoleName")?.Value ?? "";
                bool isManager = User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Tenant") ||
                                roleClaim.Contains("Admin", StringComparison.OrdinalIgnoreCase) ||
                                roleClaim.Contains("Manager", StringComparison.OrdinalIgnoreCase);

                // Resolve effective month/year for StatusWiseCounts filtering
                int filterMonth = month.HasValue && month.Value >= 1 && month.Value <= 12 ? month.Value : DateTime.Now.Month;
                int filterYear = year.HasValue && year.Value >= 2000 ? year.Value : DateTime.Now.Year;

                var totalEmployees = await _context.EmployeeMasters.CountAsync(e => e.CompanyId == companyId && e.ActiveStatus == 1);

                var activeProjects = await _context.ProjectMasters.CountAsync(p => p.CompanyId == companyId && p.ActiveStatus == 1);

                var totalClients = await _context.ClientMasters.CountAsync(c => c.CompanyId == companyId && c.ActiveStatus == 1);

                var totalModules = await _context.ModuleMasters.CountAsync(m => m.CompanyId == companyId && m.ActiveStatus == 1);

                var stats = new DashboardDTO
                {
                    TotalEmployees = totalEmployees,
                    ActiveProjects = activeProjects,
                    TotalClients = totalClients,
                    TotalModules = totalModules,
                    RecentActivities = await _context.WorkLogs
                        .Include(w => w.Project)
                        .Include(w => w.Employee)
                        .Where(w => w.CompanyId == companyId && w.ActiveStatus == 1)
                        .Where(w => isManager || w.EmployeeId == employeeId)
                        .OrderByDescending(w => w.CreateDate)
                        .Take(5)
                        .Select(w => new RecentActivityDTO
                        {
                            Title = isManager ? $"Report by {w.Employee.EmployeeName}" : "Work Report Submitted",
                            Detail = $"Project: {w.Project.ProjectName}",
                            ActivityDate = w.CreateDate,
                            Type = "Report"
                        })
                        .ToListAsync(),
                    MonthlyOverview = new List<MonthlyOverviewDTO>()
                };

                // Fetch All Active Statuses from StatusMaster
                var activeStatuses = await _context.StatusMasters
                    .Where(s => s.CompanyId == companyId && s.ActiveStatus == 1)
                    .OrderBy(s => s.StatusId)
                    .ToListAsync();

                // Populate Monthly Overview (Last 5 months)
                for (int i = 4; i >= 0; i--)
                {
                    var targetDate = DateTime.Now.AddMonths(-i);
                    var monthName = targetDate.ToString("MMM");
                    var m = targetDate.Month;
                    var y = targetDate.Year;

                    var monthData = new MonthlyOverviewDTO
                    {
                        Month = monthName,
                        StatusCounts = new List<StatusCountDTO>()
                    };

                    foreach (var status in activeStatuses)
                    {
                        var workLogCount = await _context.WorkLogTasks.CountAsync(t =>
                            t.WorkLog.CompanyId == companyId &&
                            t.WorkLog.WorkDate.Month == m &&
                            t.WorkLog.WorkDate.Year == y &&
                            t.StatusId == status.StatusId &&
                            (isManager || t.WorkLog.EmployeeId == employeeId) &&
                            t.WorkLog.ActiveStatus == 1);

                        var workEntryCount = await _context.WorkEntries.CountAsync(w =>
                            w.StatusId == status.StatusId &&
                            w.ActiveStatus == 1 &&
                            w.WorkReport.ActiveStatus == 1 &&
                            w.WorkReport.ReportDate.Month == m &&
                            w.WorkReport.ReportDate.Year == y &&
                            w.WorkReport.Employee.CompanyId == companyId &&
                            (isManager || w.WorkReport.EmployeeId == employeeId));

                        monthData.StatusCounts.Add(new StatusCountDTO
                        {
                            StatusId = status.StatusId,
                            StatusName = status.StatusName,
                            StatusColor = status.StatusColor ?? "#cbd5e1",
                            Count = workLogCount + workEntryCount
                        });
                    }

                    stats.MonthlyOverview.Add(monthData);
                }

                foreach (var status in activeStatuses)
                {
                    var taskCount = await _context.WorkLogTasks.CountAsync(t =>
                        t.WorkLog.CompanyId == companyId &&
                        t.WorkLog.WorkDate.Month == filterMonth &&
                        t.WorkLog.WorkDate.Year == filterYear &&
                        t.StatusId == status.StatusId &&
                        (isManager || t.WorkLog.EmployeeId == employeeId) &&
                        t.WorkLog.ActiveStatus == 1);

                    var overallWorkEntryCount = await _context.WorkEntries.CountAsync(w =>
                        w.StatusId == status.StatusId &&
                        w.ActiveStatus == 1 &&
                        w.WorkReport.ActiveStatus == 1 &&
                        w.WorkReport.ReportDate.Month == filterMonth &&
                        w.WorkReport.ReportDate.Year == filterYear &&
                        w.WorkReport.Employee.CompanyId == companyId &&
                        (isManager || w.WorkReport.EmployeeId == employeeId));

                    stats.StatusWiseCounts.Add(new StatusCountDTO
                    {
                        StatusId = status.StatusId,
                        StatusName = status.StatusName,
                        StatusColor = status.StatusColor ?? "#cbd5e1",
                        Count = taskCount + overallWorkEntryCount
                    });
                }

                return Ok(ApiResponse<DashboardDTO>.SuccessResponse(stats, "Dashboard stats retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("DetailedTasks")]
        public async Task<IActionResult> GetDetailedTasks([FromQuery] int? statusId, [FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                int companyId = CurrentCompanyId;
                int employeeId = CurrentEmployeeId;
                var roleClaim = User.FindFirst("RoleName")?.Value ?? "";
                bool isManager = User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Tenant") ||
                                roleClaim.Contains("Admin", StringComparison.OrdinalIgnoreCase) ||
                                roleClaim.Contains("Manager", StringComparison.OrdinalIgnoreCase);

                int filterMonth = month ?? DateTime.Now.Month;
                int filterYear = year ?? DateTime.Now.Year;

                bool fetchAll = !statusId.HasValue || statusId.Value == 0;

                var tasks = await _context.WorkLogTasks
                    .Include(t => t.WorkLog)
                    .ThenInclude(w => w.Project)
                    .Include(t => t.WorkLog)
                    .ThenInclude(w => w.Client)
                    .Include(t => t.Status)
                    .Include(t => t.WorkLog.Employee)
                    .Where(t => t.WorkLog.CompanyId == companyId &&
                                t.WorkLog.WorkDate.Month == filterMonth &&
                                t.WorkLog.WorkDate.Year == filterYear &&
                                (fetchAll || t.StatusId == statusId) &&
                                (isManager || t.WorkLog.EmployeeId == employeeId) &&
                                t.WorkLog.ActiveStatus == 1)
                    .Select(t => new {
                        ModuleId = 5043,
                        ProjectName = t.WorkLog.Project.ProjectName,
                        ClientName = t.WorkLog.Client.ClientName,
                        ModuleName = (string?)null,
                        Description = t.Description,
                        Date = t.WorkLog.WorkDate,
                        Status = t.Status.StatusName,
                        StatusColor = t.Status.StatusColor,
                        EmployeeName = t.WorkLog.Employee.EmployeeName
                    })
                    .ToListAsync();

                var entries = await _context.WorkEntries
                    .Include(w => w.WorkReport)
                    .Include(w => w.Project)
                    .Include(w => w.Module)
                    .Include(w => w.Status)
                    .Include(w => w.WorkReport.Employee)
                    .Where(w => w.WorkReport.Employee.CompanyId == companyId &&
                                w.WorkReport.ReportDate.Month == filterMonth &&
                                w.WorkReport.ReportDate.Year == filterYear &&
                                (fetchAll || w.StatusId == statusId) &&
                                (isManager || w.WorkReport.EmployeeId == employeeId) &&
                                w.ActiveStatus == 1 &&
                                w.WorkReport.ActiveStatus == 1)
                    .Select(w => new {
                        ModuleId = 5044,
                        ProjectName = w.Project.ProjectName,
                        ClientName = (string?)null,
                        ModuleName = w.Module != null ? w.Module.ModuleName : null,
                        Description = w.Description ?? w.Title,
                        Date = new DateTime(w.WorkReport.ReportDate.Year, w.WorkReport.ReportDate.Month, w.WorkReport.ReportDate.Day),
                        Status = w.Status.StatusName,
                        StatusColor = w.Status.StatusColor,
                        EmployeeName = w.WorkReport.Employee.EmployeeName
                    })
                    .ToListAsync();

                var allTasks = tasks.Concat(entries)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                return Ok(ApiResponse<object>.SuccessResponse(allTasks, "Detailed tasks retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
