using AutoMapper;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MailTemplateController : BaseApiController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MailTemplateController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("GetTemplate/{companyId}/{employeeId?}")]
    public async Task<IActionResult> GetTemplate(int companyId, int? employeeId, [FromQuery] string? templateName)
    {
        IQueryable<MailTemplate> query = _context.MailTemplates
            .Where(x => x.ActiveStatus == 1);

        if (!string.IsNullOrEmpty(templateName))
        {
            query = query.Where(x => x.TemplateName == templateName);
        }

        MailTemplate? templateObj = null;
        if (employeeId.HasValue)
        {
            templateObj = await query
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        if (templateObj == null)
        {
            templateObj = await query
                .Where(x => x.CompanyId == companyId && x.EmployeeId == null)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        var dto = _mapper.Map<MailTemplateDTO>(templateObj);
        return Ok(ApiResponse<MailTemplateDTO>.SuccessResponse(dto, "Template retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var template = await _context.MailTemplates.FindAsync(id);
        if (template == null) return NotFound(ApiResponse<MailTemplateDTO>.ErrorResponse("Template not found"));

        var dto = _mapper.Map<MailTemplateDTO>(template);
        return Ok(ApiResponse<MailTemplateDTO>.SuccessResponse(dto, "Template retrieved successfully"));
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] MailTemplateDTO dto)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        // Intelligent Upsert: Avoid duplicates for the same name and employee
        MailTemplate? existing = null;
        if (dto.Id > 0)
        {
            existing = await _context.MailTemplates.FindAsync(dto.Id);
        }
        else 
        {
            existing = await _context.MailTemplates
                .Where(x => x.TemplateName == dto.TemplateName && 
                            x.EmployeeId == dto.EmployeeId && 
                            x.CompanyId == dto.CompanyId &&
                            x.ActiveStatus == 1)
                .FirstOrDefaultAsync();
        }

        if (existing == null)
        {
            var template = _mapper.Map<MailTemplate>(dto);
            template.CreatedById = CurrentUserName;
            template.CreateDate = DateTime.Now;
            template.Guids = Guid.NewGuid();
            template.ActiveStatus = 1;
            
            _context.MailTemplates.Add(template);
        }
        else
        {
            _mapper.Map(dto, existing);
            existing.ModifiedById = CurrentUserName;
            existing.ModifiedDate = DateTime.Now;
            _context.Entry(existing).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Template saved successfully"));
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var template = await _context.MailTemplates.FindAsync(id);
        if (template == null) return NotFound(ApiResponse<string>.ErrorResponse("Template not found"));

        template.ActiveStatus = 0;
        template.ModifiedById = CurrentUserName;
        template.ModifiedDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Template deleted successfully"));
    }
}
