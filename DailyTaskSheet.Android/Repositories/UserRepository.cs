using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;
using DailyTaskSheet.App.SQLite;

namespace DailyTaskSheet.App.Repositories
{
    /// <summary>
    /// Repository for user data operations.
    /// Provides upsert logic for maintaining the current user record.
    /// </summary>
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UserRepository"/>.
        /// </summary>
        public UserRepository(DatabaseService dbService) : base(dbService) { }

        /// <inheritdoc />
        public async Task<UserModel?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FirstOrDefaultAsync(u => u.EmployeeId == employeeId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> UpsertAsync(UserModel user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var existing = await GetByEmployeeIdAsync(user.EmployeeId, cancellationToken);
            if (existing != null)
            {
                user.Id = existing.Id;
                user.CreatedAt = existing.CreatedAt;
                user.UpdatedAt = System.DateTime.UtcNow;
                return await UpdateAsync(user, cancellationToken);
            }
            return await InsertAsync(user, cancellationToken);
        }
    }
}
