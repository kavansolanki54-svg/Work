using System.Threading.Tasks;
using DallyWorkReoprt.DTO.Models;

namespace DallyWorkReoprt.Services.Interfaces;

public interface ICallLogService
{
    Task<int> SyncCallLogsAsync(CallLogSyncRequestDto request, string currentUserName);
    Task<System.Collections.Generic.IEnumerable<PhoneCallLogResponseDto>> GetAllAsync(int? employeeId);
}
