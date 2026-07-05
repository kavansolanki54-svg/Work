using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Repository
{
    public class AuthenticateRepository : GenericRepository<EmployeeMaster>, IAuthenticateRepository
    {
        public AuthenticateRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        public async Task<bool> IsValidEmployeeAsync(string Email, string Password, byte? ActiveStatus = null)
        {
            IQueryable<EmployeeMaster> query = _context.EmployeeMasters.Where(w => w.Email == Email && w.Passwords == Password);

            if (ActiveStatus.HasValue)
            {
                query = query.Where(w => w.ActiveStatus == ActiveStatus);
            }

            return await query.AnyAsync();
        }
        public IQueryable<EmployeeMaster> GetEmployeeQueryable(string Email, string Password, byte? ActiveStatus = null)
        {
            IQueryable<EmployeeMaster> query = _context.EmployeeMasters
                .Include(e => e.RoleMaster)
                    .ThenInclude(r => r!.RoleType)
                .Where(w => w.Email == Email && w.Passwords == Password);

            if (ActiveStatus.HasValue)
            {
                query = query.Where(w => w.ActiveStatus == ActiveStatus);
            }

            return query;
        }

        public RefreshToken Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();
            return refreshToken;
        }
    }
}

