using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface
{
    public interface ICommonRepository
    {
        //IQueryable<BranchMaster> GetBranches(int CompanyId, byte? ActiveStatus = null);
        IQueryable<CountryMaster> GetCountries();
        IQueryable<StateMaster> GetState(int CountryId);
        IQueryable<StateMaster> GetStates();
        IQueryable<Lookup> GetLookUps();
        //IQueryable<Lookup> Get(LookupTypeEnum TypeId, bool ActiveStatus, bool TypeActiveStatus);
    }
}

