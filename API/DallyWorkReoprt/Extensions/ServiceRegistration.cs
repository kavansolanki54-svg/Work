using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Repository;
using DallyWorkReoprt.Mappings;
using DallyWorkReoprt.Services.Interfaces;

namespace DallyWorkReoprt.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ICompanyMasterRepository, CompanyMasterRepository>();
            services.AddScoped<IAuthenticateRepository, AuthenticateRepository>();
            services.AddScoped<IRoleMasterRepository, RoleMasterRepository>();
            services.AddScoped<ISoftwareModulesRepository, SoftwareModulesRepository>();
            services.AddScoped<ICommonRepository, CommonRepository>();
            services.AddScoped<IRoleMasterSoftwareModulesRepository, RoleMasterSoftwareModulesRepository>();
            services.AddScoped<IProjectMasterRepository, ProjectMasterRepository>();
            services.AddScoped<IStatusMasterRepository, StatusMasterRepository>();
            services.AddScoped<IClientMasterRepository, ClientMasterRepository>();
            services.AddScoped<IModuleMasterRepository, ModuleMasterRepository>();
            services.AddScoped<IEmailRecipientRepository, EmailRecipientRepository>();
            services.AddScoped<IEmailSettingRepository, EmailSettingRepository>();
            services.AddScoped<IWorkReportRepository, WorkReportRepository>();
            services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();
            services.AddScoped<IReportService, DallyWorkReoprt.Services.Implementations.ReportService>();
            services.AddScoped<IEmailService, DallyWorkReoprt.Services.Implementations.EmailService>();
            services.AddScoped<IWorkLogRepository, WorkLogRepository>();
            services.AddScoped<IWorkLogService, DallyWorkReoprt.Services.Implementations.WorkLogService>();
            services.AddScoped<ICallLogService, DallyWorkReoprt.Services.Implementations.CallLogService>();

            // Register AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles));
        }
    }
}

