using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Enums;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;
using DailyTaskSheet.App.SQLite;

namespace DailyTaskSheet.App.Repositories
{
    /// <summary>
    /// Repository for call log data operations.
    /// Provides sync-specific queries, duplicate detection, and statistics.
    /// </summary>
    public class CallLogRepository : BaseRepository<CallLogModel>, ICallLogRepository
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CallLogRepository"/>.
        /// </summary>
        public CallLogRepository(DatabaseService dbService) : base(dbService) { }

        /// <inheritdoc />
        public async Task<CallLogModel?> GetByRawCallLogIdAsync(long rawCallLogId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FirstOrDefaultAsync(c => c.RawCallLogId == rawCallLogId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsByHashAsync(string syncHash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await AnyAsync(c => c.SyncHash == syncHash, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<CallLogModel>> GetByStatusAsync(SyncStatusEnum status, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int statusInt = (int)status;
            return await FindAsync(c => c.SyncStatus == statusInt, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<CallLogModel>> GetPendingForSyncAsync(int batchSize, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int pendingStatus = (int)SyncStatusEnum.Pending;
            int failedStatus = (int)SyncStatusEnum.Failed;
            var results = await Db.Table<CallLogModel>()
                .Where(c => c.SyncStatus == pendingStatus || c.SyncStatus == failedStatus)
                .OrderBy(c => c.CallDate)
                .Take(batchSize)
                .ToListAsync();
            return results;
        }

        /// <inheritdoc />
        public async Task<int> UpdateSyncStatusAsync(IEnumerable<int> ids, SyncStatusEnum status, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int statusInt = (int)status;
            var now = DateTime.UtcNow;
            int count = 0;

            foreach (var id in ids)
            {
                var result = await Db.ExecuteAsync(
                    "UPDATE CallLogs SET SyncStatus = ?, ModifiedAt = ? WHERE Id = ?",
                    statusInt, now, id);
                count += result;
            }

            return count;
        }

        /// <inheritdoc />
        public async Task<List<CallLogModel>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FindAsync(c => c.CallDate >= from && c.CallDate <= to, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> GetTodayCallCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var todayStart = DateTime.UtcNow.Date;
            var todayEnd = todayStart.AddDays(1);
            return await CountAsync(c => c.CallDate >= todayStart && c.CallDate < todayEnd, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int pendingStatus = (int)SyncStatusEnum.Pending;
            return await CountAsync(c => c.SyncStatus == pendingStatus, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> GetFailedCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int failedStatus = (int)SyncStatusEnum.Failed;
            return await CountAsync(c => c.SyncStatus == failedStatus, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<long> GetLastProcessedRawIdAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var result = await Db.ExecuteScalarAsync<long>(
                    "SELECT COALESCE(MAX(RawCallLogId), 0) FROM CallLogs");
                return result;
            }
            catch
            {
                return 0;
            }
        }

        /// <inheritdoc />
        public async Task<List<CallLogModel>> GetRecentAsync(int count, int offset = 0, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Db.Table<CallLogModel>()
                .OrderByDescending(c => c.CallDate)
                .Skip(offset)
                .Take(count)
                .ToListAsync();
        }
    }
}
