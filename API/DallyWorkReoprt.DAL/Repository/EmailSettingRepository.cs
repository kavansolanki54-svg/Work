using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository;

public class EmailSettingRepository : GenericRepository<EmailSetting>, IEmailSettingRepository
{
    public EmailSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<EmailSetting?> GetLatestAsync(int employeeId)
    {
        return await _context.EmailSettings
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.EmailSettingsId)
            .FirstOrDefaultAsync();
    }
}
