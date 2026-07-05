using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Repository interface for application settings operations.
    /// </summary>
    public interface ISettingsRepository : IRepository<SettingsModel>
    {
        /// <summary>Retrieves a setting value by key.</summary>
        Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>Sets a setting value, creating or updating as needed.</summary>
        Task SetValueAsync(string key, string value, string description = "", CancellationToken cancellationToken = default);

        /// <summary>Removes a setting by key.</summary>
        Task<int> RemoveByKeyAsync(string key, CancellationToken cancellationToken = default);
    }
}
