using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Utilities.Helper;
using AutoMapper;

namespace DallyWorkReoprt.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<LoginDTO, EmployeeMaster>().ReverseMap();
            CreateMap<RefreshTokenDTO, RefreshToken>().ReverseMap();
            CreateMap<CompanyMaster, CompanyMasterDTO>().ReverseMap();
            CreateMap<RoleMaster, RoleMasterDTO>().ReverseMap();
            CreateMap<Lookup, LookupDTO>().ReverseMap();

            CreateMap<EmployeeMaster, EmployeeMasterDTO>()
                .ForMember(d => d.IsAllowLogin, o => o.MapFrom(src => src.IsAllowLogin == 1))
                .ForMember(d => d.Passwords, o => o.MapFrom(src => new EncryptionHelper().Decrypt(src.Passwords)))
                .ReverseMap()
                .ForMember(d => d.IsAllowLogin, o => o.MapFrom(src => src.IsAllowLogin ? (byte)1 : (byte)0))
                .ForMember(d => d.Passwords, o => o.MapFrom(src => new EncryptionHelper().Encrypt(src.Passwords)));

            CreateMap<LookupType, LookupTypeDTO>().ReverseMap();
            CreateMap<ProjectMaster, ProjectDTO>().ReverseMap();
            CreateMap<StatusMaster, StatusDTO>().ReverseMap();
            CreateMap<ClientMaster, ClientDTO>().ReverseMap();
            CreateMap<ModuleMaster, ModuleDTO>().ReverseMap();
            CreateMap<EmailRecipient, EmailRecipientDTO>().ReverseMap();
            CreateMap<EmailSetting, EmailSettingDTO>().ReverseMap();

            CreateMap<WorkReport, ReportResponseDto>()
                .ForMember(d => d.Works, o => o.MapFrom(s => s.WorkEntries))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreateDate))
                .ForMember(d => d.UpdatedAt, o => o.MapFrom(s => s.ModifiedDate))
                .ForMember(d => d.ReportDate, o => o.MapFrom(s => s.ReportDate.ToDateTime(TimeOnly.MinValue)))
                .ReverseMap()
                .ForMember(d => d.ReportDate, o => o.MapFrom(s => DateOnly.FromDateTime(s.ReportDate)));

            CreateMap<WorkEntry, WorkEntryResponseDto>()
                .ForMember(d => d.TimeLogs, o => o.MapFrom(s => s.TimeLogs))
                .ReverseMap();

            CreateMap<WorkTimeLog, TimeLogResponseDto>()
                .AfterMap((src, dest) => {
                    if (TimeSpan.TryParse(src.InTime, out var inTime) && TimeSpan.TryParse(src.OutTime, out var outTime))
                    {
                        var diff = outTime - inTime;
                        if (diff.TotalMinutes < 0) diff = diff.Add(TimeSpan.FromDays(1)); // Handle overnight
                        
                        var totalMin = (int)diff.TotalMinutes;
                        if (src.Is30MinBreak && totalMin >= 30) totalMin -= 30;
                        
                        dest.TotalMinutes = totalMin;
                        dest.Hours = totalMin / 60;
                        dest.Minutes = totalMin % 60;
                        dest.DecimalHours = Math.Round(totalMin / 60m, 2);
                    }
                })
                .ReverseMap();
            
            CreateMap<ReportCreateDto, WorkReport>()
                .ForMember(d => d.WorkEntries, o => o.MapFrom(s => s.Works))
                .ForMember(d => d.ReportDate, o => o.MapFrom(s => DateOnly.FromDateTime(s.ReportDate)));

            CreateMap<WorkEntryCreateDto, WorkEntry>()
                .ForMember(d => d.TimeLogs, o => o.MapFrom(s => s.TimeLogs));

            CreateMap<MailTemplate, MailTemplateDTO>().ReverseMap();

            CreateMap<TimeLogCreateDto, WorkTimeLog>();

            CreateMap<WorkLog, WorkLogResponseDto>()
                .ForMember(d => d.Tasks, o => o.MapFrom(s => s.WorkLogTasks))
                .ReverseMap();
            CreateMap<WorkLogTask, WorkLogTaskResponseDto>().ReverseMap();
            CreateMap<WorkLogCreateDto, WorkLog>()
                .ForMember(d => d.WorkLogTasks, o => o.MapFrom(s => s.Tasks));
            CreateMap<WorkLogTaskDto, WorkLogTask>();
        }

    }
}

