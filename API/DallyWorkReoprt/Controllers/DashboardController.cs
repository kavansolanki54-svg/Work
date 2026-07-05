using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetStats()
        {
            try
            {
                int companyId = CurrentCompanyId;

                // 1. Total Employees (active)
                var totalEmployees = await _context.EmployeeMasters
                    .CountAsync(e => e.CompanyId == companyId && e.ActiveStatus == 1);

                // 2. Active Projects
                var activeProjects = await _context.ProjectMasters
                    .CountAsync(p => p.CompanyId == companyId && p.ActiveStatus == 1);

                // 3. Total Clients
                var totalClients = await _context.ClientMasters
                    .CountAsync(c => c.CompanyId == companyId && c.ActiveStatus == 1);

                // 4. Reports Pending (For today, if any sessions haven't been started or completed)
                // This is a simple count of WorkLogs for today
                var today = DateTime.Today;
                var logsCompletedToday = await _context.WorkLogs
                    .CountAsync(w => w.CompanyId == companyId && w.WorkDate.Date == today && w.ActiveStatus == 1);

                var stats = new DashboardDTO
                {
                    TotalEmployees = totalEmployees,
                    ActiveProjects = activeProjects,
                    TotalClients = totalClients,
                    ReportsPending = logsCompletedToday == 0 ? 1 : 0, // Simplified logic for demo
                    RecentActivities = await _context.WorkLogs
                        .Include(w => w.Project)
                        .Include(w => w.Employee)
                        .Where(w => w.CompanyId == companyId && w.ActiveStatus == 1)
                        .OrderByDescending(w => w.CreateDate)
                        .Take(5)
                        .Select(w => new RecentActivityDTO
                        {
                            Title = $"Report by {w.Employee.EmployeeName}",
                            Detail = $"Project: {w.Project.ProjectName}",
                            ActivityDate = w.CreateDate,
                            Type = "Report"
                        })
                        .ToListAsync()
                };

                return Ok(ApiResponse<DashboardDTO>.SuccessResponse(stats, "Dashboard stats retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
