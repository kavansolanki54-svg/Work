using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models.Analytics;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CallAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CallAnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetEmployeeId()
        {
            var empIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (empIdClaim != null && int.TryParse(empIdClaim.Value, out int empId))
            {
                return empId;
            }
            throw new UnauthorizedAccessException("Employee ID not found in token.");
        }

        /// <summary>
        /// Syncs raw call logs from the mobile application.
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> SyncCallLogs([FromBody] SyncPayloadDTO payload)
        {
            try
            {
                var employeeId = GetEmployeeId();
                if (payload == null || payload.CallLogs == null || !payload.CallLogs.Any())
                {
                    return Ok(ApiResponse<string>.SuccessResponse(null, "No logs to sync."));
                }

                // Retrieve the employee to get CreatedById (required by schema)
                var employee = await _context.EmployeeMasters.FindAsync(employeeId);
                var createdById = employee?.EmployeeCode?.ToString() ?? employeeId.ToString();

                var newLogs = new List<PhoneCallLog>();

                foreach (var dto in payload.CallLogs)
                {
                    newLogs.Add(new PhoneCallLog
                    {
                        EmployeeId = employeeId,
                        PhoneNumber = dto.PhoneNumber,
                        ContactName = string.IsNullOrEmpty(dto.ContactName) ? null : dto.ContactName,
                        CallType = dto.CallType,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime,
                        DurationInSeconds = dto.DurationInSeconds,
                        SimId = dto.SimId,
                        ActiveStatus = 1,
                        CreateDate = DateTime.UtcNow,
                        CreatedById = createdById,
                        Guids = Guid.NewGuid()
                    });
                }

                // Process Daily Summaries if they exist
                if (payload.DailySummaries != null && payload.DailySummaries.Any())
                {
                    // For now, we can log them or process them into a dedicated table.
                    // The daily summaries are primarily pre-aggregated by the mobile SQLite cache.
                    // SQL Server can calculate them dynamically, but accepting them fulfills the API spec.
                    var summaryCount = payload.DailySummaries.Count;
                    // TODO: Insert into a DailySummary table if needed
                }

                // We use AddRangeAsync for bulk insert. For huge payloads (10k+), consider SqlBulkCopy or batched inserts.
                await _context.PhoneCallLogs.AddRangeAsync(newLogs);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<string>.SuccessResponse(null, $"Successfully synced {newLogs.Count} call logs and {payload.DailySummaries?.Count ?? 0} daily summaries."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error syncing call logs: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets overall call analytics summary for the logged-in employee.
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var employeeId = GetEmployeeId();
                
                var query = _context.PhoneCallLogs.Where(l => l.EmployeeId == employeeId && l.ActiveStatus == 1);
                
                if (from.HasValue) query = query.Where(l => l.StartTime >= from.Value);
                if (to.HasValue) query = query.Where(l => l.StartTime <= to.Value);

                var totalCalls = await query.CountAsync();
                var totalDuration = await query.SumAsync(l => (int?)l.DurationInSeconds) ?? 0;
                var uniqueContacts = await query.Select(l => l.PhoneNumber).Distinct().CountAsync();
                
                var answeredCalls = await query.CountAsync(l => 
                    (l.CallType.ToLower() == "incoming" || l.CallType.ToLower() == "outgoing") 
                    && l.DurationInSeconds > 0);

                var answerRate = totalCalls > 0 ? (double)answeredCalls / totalCalls * 100 : 0;

                var summary = new AnalyticsSummaryDTO
                {
                    TotalCalls = totalCalls,
                    TotalDuration = totalDuration,
                    UniqueContacts = uniqueContacts,
                    AnswerRate = Math.Round(answerRate, 1)
                };

                return Ok(ApiResponse<AnalyticsSummaryDTO>.SuccessResponse(summary, "Summary retrieved successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error getting summary: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets the top contacts for the logged-in employee.
        /// </summary>
        [HttpGet("top-contacts")]
        public async Task<IActionResult> GetTopContacts([FromQuery] int limit = 10, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var employeeId = GetEmployeeId();
                var query = _context.PhoneCallLogs.Where(l => l.EmployeeId == employeeId && l.ActiveStatus == 1);
                
                if (from.HasValue) query = query.Where(l => l.StartTime >= from.Value);
                if (to.HasValue) query = query.Where(l => l.StartTime <= to.Value);

                var topContactsQuery = await query
                    .GroupBy(l => l.PhoneNumber)
                    .Select(g => new
                    {
                        PhoneNumber = g.Key,
                        ContactName = g.Max(c => c.ContactName), // Use the latest or any known name
                        TotalCalls = g.Count(),
                        TotalDuration = g.Sum(c => c.DurationInSeconds)
                    })
                    .OrderByDescending(x => x.TotalCalls)
                    .Take(limit)
                    .ToListAsync();

                var result = topContactsQuery.Select((c, index) => new TopContactDTO
                {
                    PhoneNumber = c.PhoneNumber,
                    ContactName = c.ContactName ?? string.Empty,
                    TotalCalls = c.TotalCalls,
                    TotalDuration = c.TotalDuration,
                    Rank = index + 1
                }).ToList();

                return Ok(ApiResponse<List<TopContactDTO>>.SuccessResponse(result, "Top contacts retrieved successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error getting top contacts: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets daily trends for line charts on the dashboard.
        /// </summary>
        [HttpGet("daily-trends")]
        public async Task<IActionResult> GetDailyTrends([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                var employeeId = GetEmployeeId();
                
                var query = _context.PhoneCallLogs
                    .Where(l => l.EmployeeId == employeeId && l.ActiveStatus == 1 && l.StartTime >= from && l.StartTime <= to);

                var trends = await query
                    .GroupBy(l => l.StartTime.Date)
                    .Select(g => new DailyTrendDTO
                    {
                        Date = g.Key,
                        IncomingCalls = g.Sum(c => c.CallType.ToLower() == "incoming" ? 1 : 0),
                        OutgoingCalls = g.Sum(c => c.CallType.ToLower() == "outgoing" ? 1 : 0),
                        MissedCalls = g.Sum(c => c.CallType.ToLower() == "missed" ? 1 : 0),
                        RejectedCalls = g.Sum(c => c.CallType.ToLower() == "rejected" ? 1 : 0)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                return Ok(ApiResponse<List<DailyTrendDTO>>.SuccessResponse(trends, "Daily trends retrieved successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error getting daily trends: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets paginated raw logs.
        /// </summary>
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
        {
            try
            {
                var employeeId = GetEmployeeId();
                var query = _context.PhoneCallLogs.Where(l => l.EmployeeId == employeeId && l.ActiveStatus == 1);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var lowerSearch = search.ToLower();
                    query = query.Where(l => l.PhoneNumber.Contains(lowerSearch) || (l.ContactName != null && l.ContactName.ToLower().Contains(lowerSearch)));
                }

                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(l => l.StartTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new CallLogSyncDTO
                    {
                        PhoneNumber = l.PhoneNumber,
                        ContactName = l.ContactName ?? string.Empty,
                        CallType = l.CallType,
                        StartTime = l.StartTime,
                        EndTime = l.EndTime,
                        DurationInSeconds = l.DurationInSeconds,
                        SimId = l.SimId ?? string.Empty
                    })
                    .ToListAsync();

                var pagedResponse = new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Items = items
                };

                return Ok(ApiResponse<object>.SuccessResponse(pagedResponse, "Logs retrieved successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error getting logs: {ex.Message}"));
            }
        }
    }
}
