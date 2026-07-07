using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.SQLite;

namespace Callyzer.App.Repositories
{
    /// <summary>
    /// Repository for call log CRUD operations.
    /// Extends BaseRepository with call-log-specific queries.
    /// </summary>
    public class CallLogRepository : BaseRepository<CallLogModel>
    {
        public CallLogRepository(DatabaseService dbService) : base(dbService) { }

        /// <summary>Get the highest RawCallLogId currently stored.</summary>
        public async Task<long> GetLastRawCallLogIdAsync()
        {
            var result = await Db.ExecuteScalarAsync<long>(
                "SELECT COALESCE(MAX(RawCallLogId), 0) FROM CallLogs");
            return result;
        }

        /// <summary>Get call logs that are pending sync.</summary>
        public async Task<List<CallLogModel>> GetPendingSyncAsync(int limit = 100)
        {
            return await Db.QueryAsync<CallLogModel>(
                "SELECT * FROM CallLogs WHERE SyncStatus = 0 ORDER BY CallDate ASC LIMIT ?", limit);
        }

        /// <summary>Get the count of pending sync records.</summary>
        public async Task<int> GetPendingSyncCountAsync()
        {
            return await Db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM CallLogs WHERE SyncStatus = 0");
        }

        /// <summary>Check if a call log with this hash already exists (duplicate detection).</summary>
        public async Task<bool> ExistsByHashAsync(string syncHash)
        {
            var count = await Db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM CallLogs WHERE SyncHash = ?", syncHash);
            return count > 0;
        }

        /// <summary>Batch update sync status for a list of IDs.</summary>
        public async Task BatchUpdateSyncStatusAsync(IEnumerable<int> ids, int newStatus)
        {
            var idList = string.Join(",", ids);
            if (string.IsNullOrEmpty(idList)) return;
            await Db.ExecuteAsync(
                $"UPDATE CallLogs SET SyncStatus = ?, ModifiedAt = ? WHERE Id IN ({idList})",
                newStatus, DateTime.UtcNow.ToString("o"));
        }

        /// <summary>Get total call count for a date range.</summary>
        public async Task<int> GetCountByDateRangeAsync(DateTime from, DateTime to)
        {
            return await Db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM CallLogs WHERE CallDate >= ? AND CallDate < ?",
                from.ToString("o"), to.ToString("o"));
        }

        public async Task<List<CallLogModel>> GetLogsAsync(int skip = 0, int take = 100)
        {
            return await Db.QueryAsync<CallLogModel>(
                "SELECT * FROM CallLogs ORDER BY CallDate DESC LIMIT ? OFFSET ?", take, skip);
        }

        /// <summary>Get logs by date range.</summary>
        public async Task<List<CallLogModel>> GetLogsByDateRangeAsync(DateTime from, DateTime to, System.Threading.CancellationToken ct = default)
        {
            return await Db.QueryAsync<CallLogModel>(
                "SELECT * FROM CallLogs WHERE CallDate >= ? AND CallDate < ? ORDER BY CallDate DESC",
                from.ToString("o"), to.ToString("o"));
        }
    }
}
