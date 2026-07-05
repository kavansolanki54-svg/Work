using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IEmailRecipientRepository : IGenericRepository<EmailRecipient>
{
    IQueryable<EmailRecipient> GetAll(int employeeId, bool? activeStatus = null);
}
