using DallyWorkReoprt.DAL.Models;

namespace DallyWorkReoprt.DAL.Interface;

public interface IWorkReportRepository : IGenericRepository<WorkReport>
{
    Task<IEnumerable<WorkReport>> GetFullReportsAsync(int employeeId);
    Task<WorkReport?> GetFullReportByIdAsync(int id);
}
