using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IClientMasterRepository : IGenericRepository<ClientMaster>
{
    IQueryable<ClientMaster> GetAll(int companyId, byte? activeStatus = null);
}
