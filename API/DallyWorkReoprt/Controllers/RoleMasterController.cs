using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleMasterController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public RoleMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? companyId)
        {
            var query = _context.RoleMasters.AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(r => r.CompanyId == companyId && r.ActiveStatus == 1);
            }

            var roles = await query.ToListAsync();
            return Ok(ApiResponse<List<RoleMaster>>.SuccessResponse(roles, "Roles retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _context.RoleMasters.FindAsync(id);
            if (role == null) return NotFound(ApiResponse<string>.ErrorResponse("Role not found"));

            return Ok(ApiResponse<RoleMaster>.SuccessResponse(role, "Role retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDTO model)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();

            var role = new RoleMaster
            {
                CompanyId = model.CompanyId,
                RoleName = model.RoleName,
                RoleTypeId = model.RoleTypeId,
                Descriptions = model.Descriptions,
                ActiveStatus = 1,
                CreatedById = User.Identity?.Name ?? "System",
                CreateDate = DateTime.Now,
                Guids = Guid.NewGuid()
            };

            _context.RoleMasters.Add(role);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<RoleMaster>.SuccessResponse(role, "Role created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDTO model)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();

            var role = await _context.RoleMasters.FindAsync(id);
            if (role == null) return NotFound(ApiResponse<string>.ErrorResponse("Role not found"));

            role.RoleName = model.RoleName;
            role.RoleTypeId = model.RoleTypeId;
            role.Descriptions = model.Descriptions;
            role.ModifiedById = User.Identity?.Name ?? "System";
            role.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<RoleMaster>.SuccessResponse(role, "Role updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.RoleMasters.FindAsync(id);
            if (role == null) return NotFound(ApiResponse<string>.ErrorResponse("Role not found"));
            role.ActiveStatus = 0;
            await _context.SaveChangesAsync();
            return Ok(ApiResponse<string>.SuccessResponse(null, "Role deleted successfully"));
        }
    }
}

