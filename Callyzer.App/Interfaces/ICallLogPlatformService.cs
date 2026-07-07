using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Models;

namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Platform abstraction for reading the device call log.
    /// Android: Uses ContentResolver + CallLog.Calls
    /// iOS: Uses CXCallObserver (limited to real-time observation only)
    /// </summary>
    public interface ICallLogPlatformService
    {
        /// <summary>Whether this platform supports reading the device call log history.</summary>
        bool IsCallLogAccessSupported { get; }

        /// <summary>Read new call logs since the last processed raw ID.</summary>
        Task<List<CallLogModel>> ReadNewCallLogsAsync(long lastProcessedId, CancellationToken ct = default);

        /// <summary>Read call logs within a date range (epoch millisecond timestamps).</summary>
        Task<List<CallLogModel>> ReadCallLogsByDateAsync(long fromTimestamp, long toTimestamp, CancellationToken ct = default);

        /// <summary>Get total device call log count.</summary>
        Task<int> GetTotalDeviceCallLogsAsync(CancellationToken ct = default);

        /// <summary>Request required platform permissions from the user.</summary>
        Task<bool> RequestPermissionsAsync();

        /// <summary>Whether all required permissions are currently granted.</summary>
        bool HasRequiredPermissions { get; }
    }
}
