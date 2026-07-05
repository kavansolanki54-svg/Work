using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IStatusMasterRepository : IGenericRepository<StatusMaster>
{
    IQueryable<StatusMaster> GetAll(int companyId, byte? activeStatus = null);
}
