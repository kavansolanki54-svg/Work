using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IWorkLogRepository : IGenericRepository<WorkLog>
{
    Task<IEnumerable<WorkLog>> GetLogsAsync(int companyId, int? employeeId = null);
    Task<IEnumerable<WorkLog>> GetLogsByDateAsync(int employeeId, DateTime date);
    Task<WorkLog?> GetLogByIdAsync(int id);
    Task DeleteByDateAsync(int employeeId, DateTime date);
}
