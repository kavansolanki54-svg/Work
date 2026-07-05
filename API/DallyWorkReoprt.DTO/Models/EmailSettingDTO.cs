namespace DallyWorkReoprt.DTO.Models;

public class EmailSettingDTO
{
    public int EmailSettingsId { get; set; }
    public string SmtpServer { get; set; } = null!;
    public int Port { get; set; }
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool ActiveStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? EmployeeId { get; set; }
}
