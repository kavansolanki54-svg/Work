using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Repository
{
    public class CompanyMasterRepository : GenericRepository<CompanyMaster>, ICompanyMasterRepository
    {
        public CompanyMasterRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}

