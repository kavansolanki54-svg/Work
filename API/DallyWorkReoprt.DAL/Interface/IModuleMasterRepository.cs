using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IModuleMasterRepository : IGenericRepository<ModuleMaster>
{
    IQueryable<ModuleMaster> GetAll(int companyId, byte? activeStatus = null);
}
