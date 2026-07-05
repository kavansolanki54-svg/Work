using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class MailTemplateRepository : GenericRepository<MailTemplate>, IMailTemplateRepository
{
    public MailTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MailTemplate?> GetTemplateAsync(int companyId, int? employeeId)
    {
        // Try to get employee-specific template first
        if (employeeId.HasValue)
        {
            var empTemplate = await _context.MailTemplates
                .Where(x => x.EmployeeId == employeeId && x.ActiveStatus == 1)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            
            if (empTemplate != null) return empTemplate;
        }

        // Fallback to company template where EmployeeId is NULL
        return await _context.MailTemplates
            .Where(x => x.CompanyId == companyId && x.EmployeeId == null && x.ActiveStatus == 1)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
