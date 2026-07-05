using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface
{
    public interface IRoleMasterRepository : IGenericRepository<RoleMaster>
    {
        IQueryable<RoleMaster> GetAll(int CompanyId, byte? ActiveStatus = null);
        bool RoleNameExists(int CompanyId, string RoleName, int? RoleMasterId);
    }
}

