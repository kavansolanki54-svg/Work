using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class MailTemplateRepository : GenericRepository<MailTemplate>, IMailTemplateRepository
{
    public MailTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MailTemplate?> GetTemplateAsync(int companyId, int? employeeId, string? templateName = null)
    {
        // Try to get employee-specific template first
        if (employeeId.HasValue)
        {
            var empQuery = _context.MailTemplates
                .Where(x => x.EmployeeId == employeeId && x.ActiveStatus == 1);

            if (!string.IsNullOrEmpty(templateName))
            {
                empQuery = empQuery.Where(x => x.TemplateName == templateName);
            }

            var empTemplate = await empQuery.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (empTemplate != null) return empTemplate;
        }

        // Fallback to company template where EmployeeId is NULL
        var companyQuery = _context.MailTemplates
            .Where(x => x.CompanyId == companyId && x.EmployeeId == null && x.ActiveStatus == 1);

        if (!string.IsNullOrEmpty(templateName))
        {
            companyQuery = companyQuery.Where(x => x.TemplateName == templateName);
        }

        return await companyQuery.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }
}
