using System;
using System.Linq;
using System.Threading.Tasks;
using DallyWorkReoprt.DAL.Models;
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
    public class SyncManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SyncManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Health")]
        public async Task<IActionResult> GetSyncHealth()
        {
            try
            {
                var totalDevices = await _context.DeviceInformations.CountAsync();
                
                var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);
                var activeDevices = await _context.DeviceInformations
                    .Where(d => d.LastSyncTime >= twentyFourHoursAgo)
                    .CountAsync();

                var health = new
                {
                    TotalRegisteredDevices = totalDevices,
                    ActiveDevices24H = activeDevices,
                    HealthPercentage = totalDevices > 0 ? (activeDevices / (double)totalDevices) * 100 : 100,
                    LastChecked = DateTime.UtcNow
                };

                return Ok(ApiResponse<object>.SuccessResponse(health, "Sync health retrieved."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error getting sync health: {ex.Message}"));
            }
        }

        [HttpPost("ForceSync/{deviceId}")]
        public IActionResult ForceSync(string deviceId)
        {
            try
            {
                // In a real-world scenario (e.g., using Firebase Cloud Messaging or SignalR), 
                // we would send a push notification to wake the device and force it to run SyncWorker.
                // For this demo, we simply acknowledge the command.
                
                return Ok(ApiResponse<string>.SuccessResponse(null, $"Force sync command issued for device {deviceId}. Device will sync on next network ping."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error forcing sync: {ex.Message}"));
            }
        }
    }
}
