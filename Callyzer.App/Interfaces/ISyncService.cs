using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Models;

namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Service responsible for synchronizing local call logs to the cloud API.
    /// Handles batching, exponential backoff, and state management.
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Gets the number of call logs currently pending sync.
        /// </summary>
        Task<int> GetPendingSyncCountAsync(CancellationToken ct = default);

        /// <summary>
        /// Initiates a sync operation for all unsynced logs.
        /// </summary>
        /// <param name="isBackground">Whether this sync was triggered by a background worker</param>
        Task<bool> SyncPendingLogsAsync(bool isBackground = false, CancellationToken ct = default);

        /// <summary>
        /// Checks if a sync operation is currently in progress.
        /// </summary>
        bool IsSyncing { get; }

        /// <summary>
        /// Event fired when sync status or progress changes.
        /// </summary>
        event EventHandler<SyncProgressEventArgs>? SyncProgressChanged;
    }

    public class SyncProgressEventArgs : EventArgs
    {
        public bool IsSyncing { get; set; }
        public int TotalToSync { get; set; }
        public int SyncedSoFar { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
