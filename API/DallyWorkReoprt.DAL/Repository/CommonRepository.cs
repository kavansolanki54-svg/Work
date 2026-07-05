using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository
{
    public class CommonRepository : ICommonRepository
    {
        protected readonly ApplicationDbContext _context;

        public CommonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IQueryable<Lookup> Get(LookupTypeEnum TypeId, bool ActiveStatus, bool TypeActiveStatus)
        //{
        //    IQueryable<Lookup> lookups = _context.Lookups.Where(w => w.TypeId == (short)TypeId && w.ActiveStatus == ActiveStatus && w.Type.ActiveStatus == TypeActiveStatus);

        //    return lookups;
        //}

        public IQueryable<Lookup> GetLookUps()
        {
            return _context.Lookups.Include(i => i.Type).Where(w => w.ActiveStatus == true && w.Type.ActiveStatus == true);
        }

        //public IQueryable<BranchMaster> GetBranches(int CompanyId, byte? ActiveStatus = null)
        //{
        //    var _query = _context.BranchMasters.Where(w => w.CompanyId == CompanyId);

        //    if (ActiveStatus.HasValue)
        //    {
        //        _query = _query.Where(w => w.ActiveStatus == ActiveStatus);
        //    }

        //    return _query;
        //}
        public IQueryable<CountryMaster> GetCountries()
        {
            return _context.CountryMasters.Where(w => w.ActiveStatus == 1);
        }

        public IQueryable<StateMaster> GetStates()
        {
            return _context.StateMasters.Where(w => w.ActiveStatus == 1);
        }
        public IQueryable<StateMaster> GetState(int CountryId)
        {
            return _context.StateMasters.Where(w => w.CountryId == CountryId && w.ActiveStatus == 1);
        }
    }
}

