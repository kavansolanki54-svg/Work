using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Repository interface for synchronization history and pending sync operations.
    /// </summary>
    public interface ISyncRepository : IRepository<SyncHistoryModel>
    {
        /// <summary>Retrieves recent sync history records.</summary>
        Task<List<SyncHistoryModel>> GetRecentHistoryAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>Retrieves the most recent sync history record.</summary>
        Task<SyncHistoryModel?> GetLastSyncAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves all pending sync records that are ready for retry.</summary>
        Task<List<PendingSyncModel>> GetPendingSyncsAsync(CancellationToken cancellationToken = default);

        /// <summary>Adds a pending sync record.</summary>
        Task<int> AddPendingSyncAsync(PendingSyncModel pendingSync, CancellationToken cancellationToken = default);

        /// <summary>Updates a pending sync record after a retry attempt.</summary>
        Task<int> UpdatePendingSyncAsync(PendingSyncModel pendingSync, CancellationToken cancellationToken = default);

        /// <summary>Removes completed pending sync records.</summary>
        Task<int> RemoveCompletedPendingSyncsAsync(CancellationToken cancellationToken = default);

        /// <summary>Adds a failed request for later retry.</summary>
        Task<int> AddFailedRequestAsync(FailedRequestModel failedRequest, CancellationToken cancellationToken = default);

        /// <summary>Retrieves failed requests that are ready for retry.</summary>
        Task<List<FailedRequestModel>> GetRetryableFailedRequestsAsync(CancellationToken cancellationToken = default);
    }
}
