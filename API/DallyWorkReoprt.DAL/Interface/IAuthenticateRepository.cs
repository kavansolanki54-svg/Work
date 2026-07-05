using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface
{
    public interface IAuthenticateRepository : IGenericRepository<EmployeeMaster>
    {
        public Task<bool> IsValidEmployeeAsync(string Email, string Password, byte? ActiveStatus = null);
        public IQueryable<EmployeeMaster> GetEmployeeQueryable(string Email, string Password, byte? ActiveStatus = null);
        RefreshToken Add(RefreshToken refreshToken);
    }
}

