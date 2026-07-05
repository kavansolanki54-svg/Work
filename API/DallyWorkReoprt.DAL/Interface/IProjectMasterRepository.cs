using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IProjectMasterRepository : IGenericRepository<ProjectMaster>
{
    IQueryable<ProjectMaster> GetAll(int companyId, byte? activeStatus = null);
}
