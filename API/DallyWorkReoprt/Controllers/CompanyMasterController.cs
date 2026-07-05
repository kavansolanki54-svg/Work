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
    public class CompanyMasterController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CompanyMasterController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var company = await _context.CompanyMasters
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null) return NotFound(ApiResponse<CompanyMaster>.ErrorResponse("Company not found"));

            return Ok(ApiResponse<CompanyMaster>.SuccessResponse(company, "Company retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CompanyDTO model)
        {
            if (!ModelState.IsValid) return ValidationErrorResponse();

            var company = new CompanyMaster
            {
                CompanyName = model.CompanyName,
                Email = model.Email,
                FullAddress = model.FullAddress,
                CityName = model.CityName,
                CountryId = model.CountryId,
                StateId = model.StateId,
                Pincode = model.Pincode,
                ActiveStatus = 1,
                CreateDate = DateTime.Now,
                Guids = Guid.NewGuid()
            };

            if (model.LogoFile != null)
            {
                company.LogoUrl = await SaveFile(model.LogoFile, "logos");
            }

            _context.CompanyMasters.Add(company);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<CompanyMaster>.SuccessResponse(company, "Company created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CompanyDTO model)
        {
            var company = await _context.CompanyMasters.FindAsync(id);
            if (company == null) return NotFound(ApiResponse<CompanyMaster>.ErrorResponse("Company not found"));

            company.CompanyName = model.CompanyName;
            company.Email = model.Email;
            company.FullAddress = model.FullAddress;
            company.CityName = model.CityName;
            company.CountryId = model.CountryId;
            company.StateId = model.StateId;
            company.Pincode = model.Pincode;

            if (model.LogoFile != null)
            {
                company.LogoUrl = await SaveFile(model.LogoFile, "logos");
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<CompanyMaster>.SuccessResponse(company, "Company updated successfully"));
        }

        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploads = Path.Combine(webRoot, "uploads", folder);
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }
    }
}

