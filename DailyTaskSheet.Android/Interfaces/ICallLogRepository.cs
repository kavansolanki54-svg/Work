using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Enums;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Repository interface for call log data operations.
    /// Provides specialized queries for sync, duplicate detection, and statistics.
    /// </summary>
    public interface ICallLogRepository : IRepository<CallLogModel>
    {
        /// <summary>Retrieves a call log by its raw Android call log ID.</summary>
        Task<CallLogModel?> GetByRawCallLogIdAsync(long rawCallLogId, CancellationToken cancellationToken = default);

        /// <summary>Checks if a call log with the given hash already exists.</summary>
        Task<bool> ExistsByHashAsync(string syncHash, CancellationToken cancellationToken = default);

        /// <summary>Retrieves all call logs with a specific sync status.</summary>
        Task<List<CallLogModel>> GetByStatusAsync(SyncStatusEnum status, CancellationToken cancellationToken = default);

        /// <summary>Retrieves pending call logs up to the specified batch size.</summary>
        Task<List<CallLogModel>> GetPendingForSyncAsync(int batchSize, CancellationToken cancellationToken = default);

        /// <summary>Updates the sync status of multiple call log records.</summary>
        Task<int> UpdateSyncStatusAsync(IEnumerable<int> ids, SyncStatusEnum status, CancellationToken cancellationToken = default);

        /// <summary>Retrieves call logs for a specific date range.</summary>
        Task<List<CallLogModel>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

        /// <summary>Returns the count of today's call logs.</summary>
        Task<int> GetTodayCallCountAsync(CancellationToken cancellationToken = default);

        /// <summary>Returns the count of pending (unsynced) call logs.</summary>
        Task<int> GetPendingCountAsync(CancellationToken cancellationToken = default);

        /// <summary>Returns the count of failed sync call logs.</summary>
        Task<int> GetFailedCountAsync(CancellationToken cancellationToken = default);

        /// <summary>Returns the latest raw call log ID that has been processed.</summary>
        Task<long> GetLastProcessedRawIdAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves recent call logs for display, ordered by date descending.</summary>
        Task<List<CallLogModel>> GetRecentAsync(int count, int offset = 0, CancellationToken cancellationToken = default);
    }
}
