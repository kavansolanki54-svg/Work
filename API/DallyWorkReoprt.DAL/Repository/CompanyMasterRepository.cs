using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository
{
    public class CompanyMasterRepository : GenericRepository<CompanyMaster>, ICompanyMasterRepository
    {
        public CompanyMasterRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}

