using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
    public class DeviceManagementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeviceManagementController(ApplicationDbContext context)
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

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceInformation device)
        {
            try
            {
                var employeeId = GetEmployeeId();
                if (device == null || string.IsNullOrEmpty(device.DeviceId))
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Invalid device information."));
                }

                var existingDevice = await _context.DeviceInformations.FirstOrDefaultAsync(d => d.DeviceId == device.DeviceId);

                if (existingDevice != null)
                {
                    existingDevice.EmployeeId = employeeId;
                    existingDevice.Manufacturer = device.Manufacturer;
                    existingDevice.Model = device.Model;
                    existingDevice.OsVersion = device.OsVersion;
                    existingDevice.AppVersion = device.AppVersion;
                    existingDevice.Platform = device.Platform;
                    existingDevice.BatteryPercentage = device.BatteryPercentage;
                    existingDevice.TimeZone = device.TimeZone;
                    existingDevice.LastSyncTime = DateTime.UtcNow;
                    _context.DeviceInformations.Update(existingDevice);
                }
                else
                {
                    device.EmployeeId = employeeId;
                    device.LastSyncTime = DateTime.UtcNow;
                    _context.DeviceInformations.Add(device);
                }

                await _context.SaveChangesAsync();
                return Ok(ApiResponse<string>.SuccessResponse(null, "Device registered successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error registering device: {ex.Message}"));
            }
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                // In a real application, we might filter by Company or Role.
                // For this dashboard, we will just return all devices with employee info.
                var devices = await _context.DeviceInformations
                    .Join(_context.EmployeeMasters, d => d.EmployeeId, e => e.EmployeeId, (d, e) => new
                    {
                        d.DeviceId,
                        EmployeeName = e.EmployeeName,
                        d.Manufacturer,
                        d.Model,
                        d.OsVersion,
                        d.AppVersion,
                        d.Platform,
                        d.BatteryPercentage,
                        d.TimeZone,
                        d.LastSyncTime
                    })
                    .OrderByDescending(d => d.LastSyncTime)
                    .ToListAsync();

                return Ok(ApiResponse<object>.SuccessResponse(devices, "Devices retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error getting devices: {ex.Message}"));
            }
        }
    }
}
