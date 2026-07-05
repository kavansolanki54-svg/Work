using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Services.Interfaces;
using System.Transactions;

namespace DallyWorkReoprt.Services.Implementations;

public class WorkLogService : IWorkLogService
{
    private readonly IWorkLogRepository _repo;
    private readonly IMapper _mapper;

    public WorkLogService(IWorkLogRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WorkLogResponseDto>> GetAllAsync(int companyId, int? employeeId = null)
    {
        var logs = await _repo.GetLogsAsync(companyId, employeeId);
        return _mapper.Map<IEnumerable<WorkLogResponseDto>>(logs);
    }

    public async Task<WorkLogResponseDto?> GetByIdAsync(int id)
    {
        var log = await _repo.GetLogByIdAsync(id);
        return _mapper.Map<WorkLogResponseDto>(log);
    }

    public async Task<WorkLogResponseDto> CreateAsync(WorkLogCreateDto dto, int employeeId, int companyId, string createdBy)
    {
        var (duration, minutes) = ConvertTime(dto.InputTime);

        var log = _mapper.Map<WorkLog>(dto);
        log.EmployeeId = employeeId;
        log.CompanyId = companyId;
        log.TotalDuration = duration;
        log.TotalMinutes = minutes;
        log.ActiveStatus = 1;
        log.CreatedById = createdBy;
        log.CreateDate = DateTime.Now;
        log.Guids = Guid.NewGuid();

        foreach (var task in log.WorkLogTasks)
        {
            if (task.StatusId <= 0) task.StatusId = 3; // Fix for FK constraint (3 is valid)
            task.CreatedById = createdBy;
            task.CreateDate = DateTime.Now;
            task.Guids = Guid.NewGuid();
        }

        await _repo.AddAsync(log);
        await _repo.SaveAsync();

        return _mapper.Map<WorkLogResponseDto>(log);
    }

    public async Task<bool> SaveSessionAsync(WorkReportSessionDto session, int employeeId, int companyId, string userName)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // 1. Physically clear existing logs for the date to allow clean re-save
        await _repo.DeleteByDateAsync(employeeId, session.WorkDate);

        // 2. Insert new session logs
        foreach (var logDto in session.Logs)
        {
            var (duration, minutes) = ConvertTime(logDto.InputTime);
            var log = _mapper.Map<WorkLog>(logDto);
            log.EmployeeId = employeeId;
            log.CompanyId = companyId;
            log.WorkDate = session.WorkDate.Date;
            log.TotalDuration = duration;
            log.TotalMinutes = minutes;
            log.ActiveStatus = 1;
            log.CreatedById = userName;
            log.CreateDate = DateTime.Now;
            log.Guids = Guid.NewGuid();

            foreach (var task in log.WorkLogTasks)
            {
                if (task.StatusId <= 0) task.StatusId = 3;
                task.CreatedById = userName;
                task.CreateDate = DateTime.Now;
                task.Guids = Guid.NewGuid();
            }

            await _repo.AddAsync(log);
        }

        await _repo.SaveAsync();
        scope.Complete();
        return true;
    }

    public async Task<bool> DeleteSessionByDateAsync(DateTime date, int employeeId, string userName)
    {
        await _repo.DeleteByDateAsync(employeeId, date);
        return true;
    }
    
    public async Task<IEnumerable<WorkLogResponseDto>> GetByDateAsync(DateTime date, int companyId, int employeeId)
    {
        var logs = await _repo.GetLogsByDateAsync(employeeId, date.Date);
        return _mapper.Map<IEnumerable<WorkLogResponseDto>>(logs);
    }

    public async Task<bool> UpdateAsync(int id, WorkLogCreateDto dto, string modifiedBy)
    {
        var log = await _repo.GetLogByIdAsync(id);
        if (log == null) return false;

        var (duration, minutes) = ConvertTime(dto.InputTime);

        log.ClientId = dto.ClientId;
        log.ProjectId = dto.ProjectId;
        log.WorkDate = dto.WorkDate;
        log.InputTime = dto.InputTime;
        log.TotalDuration = duration;
        log.TotalMinutes = minutes;
        log.Mode = dto.Mode;
        log.Remarks = dto.Remarks;
        log.OtherEmployeeIds = dto.OtherEmployeeIds;
        log.ModifiedById = modifiedBy;
        log.ModifiedDate = DateTime.Now;

        log.WorkLogTasks.Clear();
        foreach (var taskDto in dto.Tasks)
        {
            if (taskDto.StatusId <= 0) taskDto.StatusId = 3;
            log.WorkLogTasks.Add(new WorkLogTask
            {
                WorkLogId = log.WorkLogId,
                Description = taskDto.Description,
                StatusId = taskDto.StatusId,
                IsCompleted = taskDto.IsCompleted,
                CreatedById = modifiedBy,
                CreateDate = DateTime.Now,
                Guids = Guid.NewGuid()
            });
        }

        await _repo.UpdateAsync(log);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string modifiedBy)
    {
        var log = await _repo.GetLogByIdAsync(id);
        if (log == null) return false;

        log.ActiveStatus = 0;
        log.ModifiedById = modifiedBy;
        log.ModifiedDate = DateTime.Now;

        await _repo.UpdateAsync(log);
        await _repo.SaveAsync();
        return true;
    }

    private (decimal duration, int minutes) ConvertTime(decimal input)
    {
        int hours = (int)input;
        int mins = (int)Math.Round((input - hours) * 100);

        if (mins >= 60)
            throw new Exception($"Invalid minute format: {mins}. Must be less than 60.");

        decimal totalDuration = hours + (mins / 60m);
        int totalMinutes = (hours * 60) + mins;

        return (Math.Round(totalDuration, 2), totalMinutes);
    }
}
