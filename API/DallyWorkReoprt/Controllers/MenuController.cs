using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : BaseApiController
    {
        private readonly ISoftwareModulesRepository _softwareModules;

        public MenuController(ISoftwareModulesRepository softwareModules)
        {
            _softwareModules = softwareModules;
        }

        [HttpGet("{roleId}/{isTenant?}")]
        public async Task<IActionResult> Get(int roleId, bool isTenant = false)
        {
            var menu = await _softwareModules.GetMenuByRoleAsync(roleId, isTenant);
            return Ok(ApiResponse<object>.SuccessResponse(menu, "Menu retrieved successfully"));
        }
    }
}

