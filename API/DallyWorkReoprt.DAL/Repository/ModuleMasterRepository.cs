using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository;

public class ModuleMasterRepository : GenericRepository<ModuleMaster>, IModuleMasterRepository
{
    public ModuleMasterRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<ModuleMaster> GetAll(int companyId, byte? activeStatus = null)
    {
        var query = _context.ModuleMasters.Where(x => x.CompanyId == companyId);
        if (activeStatus.HasValue) query = query.Where(x => x.ActiveStatus == activeStatus);
        return query;
    }
}
