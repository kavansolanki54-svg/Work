using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository;

public class EmailRecipientRepository : GenericRepository<EmailRecipient>, IEmailRecipientRepository
{
    public EmailRecipientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<EmailRecipient> GetAll(int employeeId, bool? activeStatus = null)
    {
        var query = _context.EmailRecipients.Where(x => x.EmployeeId == employeeId);
        if (activeStatus.HasValue) query = query.Where(x => x.ActiveStatus == activeStatus.Value);
        return query;
    }
}
