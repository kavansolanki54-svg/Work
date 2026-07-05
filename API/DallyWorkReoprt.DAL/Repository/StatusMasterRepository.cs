using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository;

public class StatusMasterRepository : GenericRepository<StatusMaster>, IStatusMasterRepository
{
    public StatusMasterRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<StatusMaster> GetAll(int companyId, byte? activeStatus = null)
    {
        var query = _context.StatusMasters.Where(x => x.CompanyId == companyId);
        if (activeStatus.HasValue) query = query.Where(x => x.ActiveStatus == activeStatus);
        return query;
    }
}
