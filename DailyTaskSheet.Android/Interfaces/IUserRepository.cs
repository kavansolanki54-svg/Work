using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Repository interface for user data operations.
    /// </summary>
    public interface IUserRepository : IRepository<UserModel>
    {
        /// <summary>Retrieves a user by their backend employee ID.</summary>
        Task<UserModel?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);

        /// <summary>Retrieves a user by their email address.</summary>
        Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>Inserts or updates a user based on their employee ID.</summary>
        Task<int> UpsertAsync(UserModel user, CancellationToken cancellationToken = default);
    }
}
