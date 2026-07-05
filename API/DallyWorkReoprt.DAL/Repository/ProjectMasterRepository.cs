using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository;

public class ProjectMasterRepository : GenericRepository<ProjectMaster>, IProjectMasterRepository
{
    public ProjectMasterRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<ProjectMaster> GetAll(int companyId, byte? activeStatus = null)
    {
        var query = _context.ProjectMasters.Where(x => x.CompanyId == companyId);
        if (activeStatus.HasValue) query = query.Where(x => x.ActiveStatus == activeStatus);
        return query;
    }
}
