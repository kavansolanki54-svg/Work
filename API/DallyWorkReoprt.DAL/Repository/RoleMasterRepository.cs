using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository
{
    public class RoleMasterRepository : GenericRepository<RoleMaster>, IRoleMasterRepository
    {
        public RoleMasterRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        public IQueryable<RoleMaster> GetAll(int CompanyId, byte? ActiveStatus = null)
        {
            IQueryable<RoleMaster> query = _context.RoleMasters.Where(w => w.CompanyId == CompanyId);

            if (ActiveStatus.HasValue)
            {
                query = query.Where(w => w.ActiveStatus == ActiveStatus);
            }

            return query;
        }
        public bool RoleNameExists(int CompanyId, string RoleName, int? RoleMasterId)
        {
            IQueryable<RoleMaster> _query = _context.RoleMasters.Where(w => w.CompanyId == CompanyId && w.RoleName == RoleName && w.ActiveStatus == 1);

            if (RoleMasterId.HasValue && RoleMasterId.Value > 0)
            {
                _query = _query.Where(w => w.RoleMasterId != RoleMasterId);
            }

            return _query.Any();
        }
    }
}

