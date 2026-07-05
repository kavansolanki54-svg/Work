using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DallyWorkReoprt.DTO.Models;

public class CallLogDto
{
    [Required]
    public string PhoneNumber { get; set; } = null!;

    public string? ContactName { get; set; }

    [Required]
    public string CallType { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int DurationInSeconds { get; set; }

    public string? SimId { get; set; }
}

public class CallLogSyncRequestDto
{
    [Required]
    public List<CallLogDto> Logs { get; set; } = new List<CallLogDto>();

    [Required]
    public string DeviceId { get; set; } = null!;

    [Required]
    public int EmployeeId { get; set; }
}

public class PhoneCallLogResponseDto
{
    public int CallLogId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? ContactName { get; set; }
    public string CallType { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationInSeconds { get; set; }
    public string? SimId { get; set; }
    public DateTime CreateDate { get; set; }
    public string? RecordingUrl { get; set; }
}
