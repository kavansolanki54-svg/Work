using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Repository interface for API and error log operations.
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>Inserts an API log record.</summary>
        Task<int> InsertApiLogAsync(ApiLogModel log, CancellationToken cancellationToken = default);

        /// <summary>Inserts an error log record.</summary>
        Task<int> InsertErrorLogAsync(ErrorLogModel log, CancellationToken cancellationToken = default);

        /// <summary>Retrieves recent API logs.</summary>
        Task<List<ApiLogModel>> GetRecentApiLogsAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>Retrieves recent error logs.</summary>
        Task<List<ErrorLogModel>> GetRecentErrorLogsAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>Deletes logs older than the specified number of days.</summary>
        Task<int> PurgeOldLogsAsync(int olderThanDays, CancellationToken cancellationToken = default);
    }
}
