using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository
{
    public class EmployeeRepository : GenericRepository<EmployeeMaster>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public bool EmployeeCodeExists(int CompanyId, int? EmployeeCode, int? EmployeeId)
        {
            IQueryable<EmployeeMaster> _query = _context.EmployeeMasters.Where(w => w.CompanyId == CompanyId && w.EmployeeCode == EmployeeCode && w.ActiveStatus == 1);

            if (EmployeeId.HasValue && EmployeeId.Value > 0)
            {
                _query = _query.Where(w => w.EmployeeId != EmployeeId);
            }

            return _query.Any();
        }

        public IQueryable<EmployeeMaster> GetAll(int CompanyId, bool Tenant = false)
        {
            return _context.EmployeeMasters.Where(w => w.CompanyId == CompanyId && w.Tenant == Tenant && w.ActiveStatus == 1);
        }

        public IQueryable<EmployeeMaster> GetEmployeeQueryable(string Email, string Password, byte? ActiveStatus = null)
        {
            IQueryable<EmployeeMaster> query = _context.EmployeeMasters.Where(w => w.Email == Email && w.Passwords == Password);

            if (ActiveStatus.HasValue)
            {
                query = query.Where(w => w.ActiveStatus == ActiveStatus);
            }

            return query;
        }

        public bool IsTenant(int EmployeeId)
        {
            return _context.EmployeeMasters.Any(a => a.EmployeeId == EmployeeId && a.Tenant);
        }

        public async Task<bool> IsValidEmployeeAsync(string Email, string Password, byte? ActiveStatus = null)
        {
            IQueryable<EmployeeMaster> query = _context.EmployeeMasters.Where(w => w.Email == Email && w.Passwords == Password);

            if (ActiveStatus.HasValue)
            {
                query = query.Where(w => w.ActiveStatus == ActiveStatus);
            }

            return await query.AnyAsync();
        }
        public IQueryable<CompanyMaster> GetCompanyQueryable(int CompanyId, byte? ActiveStatus = null)
        {
            IQueryable<CompanyMaster> query = _context.CompanyMasters.Where(w => w.CompanyId == CompanyId);

            if (ActiveStatus.HasValue)
            {
                query = query.Where(w => w.ActiveStatus == ActiveStatus);
            }

            return query;
        }
    }
}

