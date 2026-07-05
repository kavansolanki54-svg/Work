using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected int CurrentEmployeeId
        {
            get
            {
                var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                          ?? User.FindFirst("sub")?.Value;
                return claim != null ? int.Parse(claim) : 0;
            }
        }

        protected int CurrentCompanyId
        {
            get
            {
                var claim = User.FindFirst("CompanyId")?.Value;
                return claim != null ? int.Parse(claim) : 0;
            }
        }

        protected string CurrentUserName => User.Identity?.Name ?? "System";

        protected IActionResult ValidationErrorResponse(string? message = null)
        {
            var errors = ModelState
                .Where(x => x.Value!.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Ok(ApiResponse<Type?>.ErrorResponse(message ?? "Validation Failed", errors));
        }
    }
}

