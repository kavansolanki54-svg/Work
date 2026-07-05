using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MasterDataController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public MasterDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.CountryMasters
                .Where(c => c.ActiveStatus == 1)
                .Select(c => new { c.CountryId, c.CountryName })
                .ToListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(countries, "Countries retrieved"));
        }

        [HttpGet("states/{countryId}")]
        public async Task<IActionResult> GetStates(int countryId)
        {
            var states = await _context.StateMasters
                .Where(s => s.CountryId == countryId && s.ActiveStatus == 1)
                .Select(s => new { s.StateId, s.StateName })
                .ToListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(states, "States retrieved"));
        }

        [HttpGet("lookups/{typeName}")]
        public async Task<IActionResult> GetLookups(string typeName)
        {
            var lookups = await _context.Lookups
                .Include(l => l.Type)
                .Where(l => l.Type.TypeName == typeName && l.ActiveStatus)
                .Select(l => new { l.Id, Name = l.LookupName })
                .ToListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(lookups, "Lookups retrieved"));
        }
    }
}

