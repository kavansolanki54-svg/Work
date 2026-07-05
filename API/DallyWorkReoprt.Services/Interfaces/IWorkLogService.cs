using DallyWorkReoprt.DTO.Models;

namespace DallyWorkReoprt.Services.Interfaces;

public interface IWorkLogService
{
    Task<IEnumerable<WorkLogResponseDto>> GetAllAsync(int companyId, int? employeeId = null);
    Task<WorkLogResponseDto?> GetByIdAsync(int id);
    Task<WorkLogResponseDto> CreateAsync(WorkLogCreateDto dto, int employeeId, int companyId, string createdBy);
    Task<bool> UpdateAsync(int id, WorkLogCreateDto dto, string modifiedBy);
    Task<bool> DeleteAsync(int id, string modifiedBy);
    Task<bool> SaveSessionAsync(WorkReportSessionDto session, int employeeId, int companyId, string userName);
    Task<bool> DeleteSessionByDateAsync(DateTime date, int employeeId, string userName);
    Task<IEnumerable<WorkLogResponseDto>> GetByDateAsync(DateTime date, int companyId, int employeeId);
}
