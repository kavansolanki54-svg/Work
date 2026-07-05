using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Services.Interfaces;

namespace DallyWorkReoprt.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IWorkReportRepository _reportRepo;
    private readonly IMapper _mapper;

    public ReportService(IWorkReportRepository reportRepo, IMapper mapper)
    {
        _reportRepo = reportRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReportResponseDto>> GetAllAsync(int employeeId)
    {
        var reports = await _reportRepo.GetFullReportsAsync(employeeId);
        return _mapper.Map<IEnumerable<ReportResponseDto>>(reports);
    }

    public async Task<ReportResponseDto?> GetByIdAsync(int id)
    {
        var report = await _reportRepo.GetFullReportByIdAsync(id);
        return _mapper.Map<ReportResponseDto>(report);
    }

    public async Task<ReportResponseDto> CreateAsync(ReportCreateDto dto, int employeeId, string createdBy)
    {
        var report = _mapper.Map<WorkReport>(dto);
        report.EmployeeId = employeeId;
        report.ActiveStatus = 1;
        report.CreatedById = createdBy;
        report.CreateDate = DateTime.Now;
        report.Guids = Guid.NewGuid();

        foreach (var entry in report.WorkEntries)
        {
            entry.ActiveStatus = 1;
            entry.CreatedById = createdBy;
            entry.CreateDate = DateTime.Now;
            entry.Guids = Guid.NewGuid();

            foreach (var log in entry.TimeLogs)
            {
                log.ActiveStatus = 1;
                log.CreatedById = createdBy;
                log.CreateDate = DateTime.Now;
                log.Guids = Guid.NewGuid();
            }
        }

        await _reportRepo.AddAsync(report);
        await _reportRepo.SaveAsync();

        return _mapper.Map<ReportResponseDto>(report);
    }

    public async Task<bool> UpdateAsync(int id, ReportUpdateDto dto, string modifiedBy)
    {
        var report = await _reportRepo.GetFullReportByIdAsync(id);
        if (report == null) return false;

        // Simple approach: Delete existing entries and logs, and recreate them
        // In a more production-ready system, we'd do a formal diff/patch, but this is common for simple task sheets
        report.ModifiedById = modifiedBy;
        report.ModifiedDate = DateTime.Now;
        report.ReportDate = DateOnly.FromDateTime(dto.ReportDate);

        // Clear existing entries (EF will cascade delete them)
        report.WorkEntries.Clear();

        // Re-add from DTO
        var newEntries = _mapper.Map<List<WorkEntry>>(dto.Works);
        foreach (var entry in newEntries)
        {
            entry.ActiveStatus = 1;
            entry.CreatedById = modifiedBy; // New entry from the perspective of this update
            entry.CreateDate = DateTime.Now;
            entry.Guids = Guid.NewGuid();

            foreach (var log in entry.TimeLogs)
            {
                log.ActiveStatus = 1;
                log.CreatedById = modifiedBy;
                log.CreateDate = DateTime.Now;
                log.Guids = Guid.NewGuid();
            }
            report.WorkEntries.Add(entry);
        }

        await _reportRepo.UpdateAsync(report);
        await _reportRepo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, string modifiedBy)
    {
        var report = await _reportRepo.GetByIdAsync(id);
        if (report == null) return false;

        report.ActiveStatus = 0;
        report.ModifiedById = modifiedBy;
        report.ModifiedDate = DateTime.Now;

        await _reportRepo.UpdateAsync(report);
        await _reportRepo.SaveAsync();
        return true;
    }
}
