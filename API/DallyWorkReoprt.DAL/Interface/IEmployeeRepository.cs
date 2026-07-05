using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface
{
    public interface IEmployeeRepository : IGenericRepository<EmployeeMaster>
    {
        public Task<bool> IsValidEmployeeAsync(string Email, string Password, byte? ActiveStatus = null);
        public IQueryable<EmployeeMaster> GetEmployeeQueryable(string Email, string Password, byte? ActiveStatus = null);
        IQueryable<EmployeeMaster> GetAll(int CompanyId, bool Tenant = false);
        bool IsTenant(int EmployeeId);
        //IQueryable<EmployeeMaster> GetEmployeesWithSameBranches(int EmployeeId, byte? ActiveStatus = null);
        bool EmployeeCodeExists(int CompanyId, int? EmployeeCode, int? EmployeeId);
        public IQueryable<CompanyMaster> GetCompanyQueryable(int CompanyId, byte? ActiveStatus = null);
    }
}

