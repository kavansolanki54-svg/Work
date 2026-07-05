namespace DallyWorkReoprt.Services.Interfaces;

public interface IEmailService
{
    Task SendDailyReportEmailAsync(int reportId);
    Task SendWorkLogEmailAsync(int workLogId);
    Task SendDailyWorkReportEmailAsync(int companyId, int employeeId, DateTime date);
}
