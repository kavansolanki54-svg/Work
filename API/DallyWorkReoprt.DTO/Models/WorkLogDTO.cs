using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models;

public class WorkLogTaskDto
{
    public int? WorkLogTaskId { get; set; }
    [Required]
    public string Description { get; set; } = null!;
    public int StatusId { get; set; }
    public bool IsCompleted { get; set; }
}

public class WorkLogCreateDto
{
    public int ClientId { get; set; }
    public int ProjectId { get; set; }
    [Required]
    public DateTime WorkDate { get; set; }
    [Required]
    public decimal InputTime { get; set; } // HH.MM
    [Required]
    public string Mode { get; set; } = null!;
    public int StatusId { get; set; }
    public string? Remarks { get; set; }
    public string? OtherEmployeeIds { get; set; }
    public List<WorkLogTaskDto> Tasks { get; set; } = new();
}

public class WorkReportSessionDto
{
    [Required]
    public DateTime WorkDate { get; set; }
    public List<WorkLogCreateDto> Logs { get; set; } = new();
}

public class WorkLogResponseDto
{
    public int WorkLogId { get; set; }
    public int EmployeeId { get; set; }
    public int CompanyId { get; set; }
    public int ClientId { get; set; }
    public int ProjectId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal InputTime { get; set; }
    public decimal TotalDuration { get; set; }
    public int TotalMinutes { get; set; }
    public string Mode { get; set; } = null!;
    public string? Remarks { get; set; }
    public byte ActiveStatus { get; set; }
    public DateTime CreateDate { get; set; }
    public string? OtherEmployeeIds { get; set; }
    public int StatusId { get; set; }

    public ClientDTO? Client { get; set; }
    public ProjectDTO? Project { get; set; }
    public StatusDTO? Status { get; set; }
    public List<WorkLogTaskResponseDto> Tasks { get; set; } = new();
}

public class WorkLogTaskResponseDto
{
    public int WorkLogTaskId { get; set; }
    public string Description { get; set; } = null!;
    public int StatusId { get; set; }
    public bool IsCompleted { get; set; }
    public StatusDTO? Status { get; set; }
}
