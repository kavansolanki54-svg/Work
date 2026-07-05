using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for orchestrating call log synchronization.
    /// Coordinates reading, storing, uploading, and status tracking.
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Executes a full synchronization cycle: read new logs, store locally, upload to API.
        /// </summary>
        /// <param name="triggerSource">What triggered this sync (Manual, WorkManager, Boot, etc.).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The sync result containing counts and status.</returns>
        Task<SyncHistoryModel> ExecuteSyncAsync(string triggerSource, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retries previously failed sync operations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of successfully retried records.</returns>
        Task<int> RetryFailedSyncsAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets whether a sync is currently in progress.</summary>
        bool IsSyncing { get; }
    }
}
