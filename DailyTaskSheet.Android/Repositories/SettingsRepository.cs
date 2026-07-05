using System;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;
using DailyTaskSheet.App.SQLite;

namespace DailyTaskSheet.App.Repositories
{
    /// <summary>
    /// Repository for application settings key-value operations.
    /// Supports upsert semantics for settings management.
    /// </summary>
    public class SettingsRepository : BaseRepository<SettingsModel>, ISettingsRepository
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SettingsRepository"/>.
        /// </summary>
        public SettingsRepository(DatabaseService dbService) : base(dbService) { }

        /// <inheritdoc />
        public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var setting = await FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
            return setting?.Value;
        }

        /// <inheritdoc />
        public async Task SetValueAsync(string key, string value, string description = "", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var existing = await FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(description))
                {
                    existing.Description = description;
                }
                await UpdateAsync(existing, cancellationToken);
            }
            else
            {
                await InsertAsync(new SettingsModel
                {
                    Key = key,
                    Value = value,
                    Description = description,
                    UpdatedAt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<int> RemoveByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var setting = await FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
            if (setting != null)
            {
                return await DeleteAsync(setting, cancellationToken);
            }
            return 0;
        }
    }
}
