using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for reading call logs from the Android call log provider.
    /// Reads via ContentResolver from CallLog.Calls.ContentUri.
    /// </summary>
    public interface ICallLogReaderService
    {
        /// <summary>
        /// Reads new call logs from the Android system that haven't been captured yet.
        /// </summary>
        /// <param name="lastProcessedId">The last raw call log ID that was processed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of new call log records.</returns>
        Task<List<CallLogModel>> ReadNewCallLogsAsync(long lastProcessedId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads all call logs from the Android system within a date range.
        /// </summary>
        /// <param name="fromTimestamp">Start timestamp in milliseconds since epoch.</param>
        /// <param name="toTimestamp">End timestamp in milliseconds since epoch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of call log records.</returns>
        Task<List<CallLogModel>> ReadCallLogsByDateAsync(long fromTimestamp, long toTimestamp, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of call logs available on the device.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The total count of device call logs.</returns>
        Task<int> GetTotalDeviceCallLogsAsync(CancellationToken cancellationToken = default);
    }
}
