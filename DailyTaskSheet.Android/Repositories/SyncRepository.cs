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
    /// Repository for sync history, pending sync, and failed request operations.
    /// Manages the retry queue and sync tracking.
    /// </summary>
    public class SyncRepository : BaseRepository<SyncHistoryModel>, ISyncRepository
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SyncRepository"/>.
        /// </summary>
        public SyncRepository(DatabaseService dbService) : base(dbService) { }

        /// <inheritdoc />
        public async Task<List<SyncHistoryModel>> GetRecentHistoryAsync(int count, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Db.Table<SyncHistoryModel>()
                .OrderByDescending(s => s.SyncTime)
                .Take(count)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<SyncHistoryModel?> GetLastSyncAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var results = await Db.Table<SyncHistoryModel>()
                .OrderByDescending(s => s.SyncTime)
                .Take(1)
                .ToListAsync();
            return results.Count > 0 ? results[0] : null;
        }

        /// <inheritdoc />
        public async Task<List<PendingSyncModel>> GetPendingSyncsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var now = DateTime.UtcNow;
            return await Db.Table<PendingSyncModel>()
                .Where(p => p.Status == 0 && (p.NextAttempt == null || p.NextAttempt <= now) && p.RetryCount < p.MaxRetries)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<int> AddPendingSyncAsync(PendingSyncModel pendingSync, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Db.InsertAsync(pendingSync);
        }

        /// <inheritdoc />
        public async Task<int> UpdatePendingSyncAsync(PendingSyncModel pendingSync, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            pendingSync.UpdatedAt = DateTime.UtcNow;
            return await Db.UpdateAsync(pendingSync);
        }

        /// <inheritdoc />
        public async Task<int> RemoveCompletedPendingSyncsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Status 2 = Completed
            return await Db.ExecuteAsync("DELETE FROM PendingSync WHERE Status = 2");
        }

        /// <inheritdoc />
        public async Task<int> AddFailedRequestAsync(FailedRequestModel failedRequest, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Db.InsertAsync(failedRequest);
        }

        /// <inheritdoc />
        public async Task<List<FailedRequestModel>> GetRetryableFailedRequestsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var now = DateTime.UtcNow;
            return await Db.Table<FailedRequestModel>()
                .Where(f => !f.IsAbandoned && f.RetryCount < f.MaxRetries && (f.NextRetryAt == null || f.NextRetryAt <= now))
                .OrderBy(f => f.CreatedAt)
                .ToListAsync();
        }
    }
}
