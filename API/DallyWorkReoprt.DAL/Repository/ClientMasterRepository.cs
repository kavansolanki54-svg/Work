using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository;

public class ClientMasterRepository : GenericRepository<ClientMaster>, IClientMasterRepository
{
    public ClientMasterRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<ClientMaster> GetAll(int companyId, byte? activeStatus = null)
    {
        var query = _context.ClientMasters.Where(x => x.CompanyId == companyId);
        if (activeStatus.HasValue) query = query.Where(x => x.ActiveStatus == activeStatus);
        return query;
    }
}
