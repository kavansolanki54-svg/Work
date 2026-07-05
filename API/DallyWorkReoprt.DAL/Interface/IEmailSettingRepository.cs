using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IEmailSettingRepository : IGenericRepository<EmailSetting>
{
    Task<EmailSetting?> GetLatestAsync(int employeeId);
}
