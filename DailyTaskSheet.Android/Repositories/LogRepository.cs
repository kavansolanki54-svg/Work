using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;
using DailyTaskSheet.App.SQLite;

namespace DailyTaskSheet.App.Repositories
{
    /// <summary>
    /// Repository for API and error log operations.
    /// Supports log retrieval and automatic purging of old records.
    /// </summary>
    public class LogRepository : ILogRepository
    {
        private readonly DatabaseService _dbService;

        /// <summary>
        /// Initializes a new instance of <see cref="LogRepository"/>.
        /// </summary>
        public LogRepository(DatabaseService dbService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        /// <inheritdoc />
        public async Task<int> InsertApiLogAsync(ApiLogModel log, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _dbService.Database.InsertAsync(log);
        }

        /// <inheritdoc />
        public async Task<int> InsertErrorLogAsync(ErrorLogModel log, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _dbService.Database.InsertAsync(log);
        }

        /// <inheritdoc />
        public async Task<List<ApiLogModel>> GetRecentApiLogsAsync(int count, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _dbService.Database.Table<ApiLogModel>()
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<ErrorLogModel>> GetRecentErrorLogsAsync(int count, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _dbService.Database.Table<ErrorLogModel>()
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<int> PurgeOldLogsAsync(int olderThanDays, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var cutoff = DateTime.UtcNow.AddDays(-olderThanDays);
            int apiDeleted = await _dbService.Database.ExecuteAsync(
                "DELETE FROM ApiLogs WHERE CreatedAt < ?", cutoff);
            int errorDeleted = await _dbService.Database.ExecuteAsync(
                "DELETE FROM ErrorLogs WHERE CreatedAt < ?", cutoff);
            return apiDeleted + errorDeleted;
        }
    }
}
