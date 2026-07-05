using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte ActiveStatus { get; set; }
    public string? Color { get; set; }
}

public class StatusDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte ActiveStatus { get; set; }
    public string? Color { get; set; }
}

public class TimeLogCreateDto
{
    [Required]
    public string InTime { get; set; } = null!;
    [Required]
    public string OutTime { get; set; } = null!;
    public bool Is30MinBreak { get; set; }
}

public class TimeLogResponseDto
{
    public int Id { get; set; }
    public string InTime { get; set; } = null!;
    public string OutTime { get; set; } = null!;
    public int TotalMinutes { get; set; }
    public decimal DecimalHours { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public bool Is30MinBreak { get; set; }
    public byte ActiveStatus { get; set; }
}

public class WorkEntryCreateDto
{
    public int SrNo { get; set; }
    [Required]
    public string Title { get; set; } = null!;
    public int ProjectId { get; set; }
    public int StatusId { get; set; }
    public int? ModuleId { get; set; }
    public string? Description { get; set; }
    public List<TimeLogCreateDto> TimeLogs { get; set; } = new();
}

public class WorkEntryResponseDto
{
    public int Id { get; set; }
    public int SrNo { get; set; }
    public string Title { get; set; } = null!;
    public int ProjectId { get; set; }
    public int StatusId { get; set; }
    public int? ModuleId { get; set; }
    public ProjectDTO Project { get; set; } = null!;
    public StatusDTO Status { get; set; } = null!;
    public ModuleDTO? Module { get; set; }
    public string? Description { get; set; }
    public List<TimeLogResponseDto> TimeLogs { get; set; } = new();
}

public class ReportCreateDto
{
    [Required]
    public DateTime ReportDate { get; set; }
    public List<WorkEntryCreateDto> Works { get; set; } = new();
}

public class ReportUpdateDto
{
    [Required]
    public DateTime ReportDate { get; set; }
    public List<WorkEntryCreateDto> Works { get; set; } = new();
}

public class ReportResponseDto
{
    public int Id { get; set; }
    public DateTime ReportDate { get; set; }
    public List<WorkEntryResponseDto> Works { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
