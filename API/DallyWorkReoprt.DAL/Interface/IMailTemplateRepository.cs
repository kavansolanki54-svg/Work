using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IMailTemplateRepository : IGenericRepository<MailTemplate>
{
    Task<MailTemplate?> GetTemplateAsync(int companyId, int? employeeId, string? templateName = null);
}
