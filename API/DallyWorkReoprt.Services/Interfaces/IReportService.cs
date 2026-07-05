using DallyWorkReoprt.DTO.Models;

namespace DallyWorkReoprt.Services.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportResponseDto>> GetAllAsync(int employeeId);
    Task<ReportResponseDto?> GetByIdAsync(int id);
    Task<ReportResponseDto> CreateAsync(ReportCreateDto dto, int employeeId, string createdBy);
    Task<bool> UpdateAsync(int id, ReportUpdateDto dto, string modifiedBy);
    Task<bool> DeleteAsync(int id, string modifiedBy);
}
