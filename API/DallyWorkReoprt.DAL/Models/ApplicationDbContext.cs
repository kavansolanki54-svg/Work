using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ClientMaster> ClientMasters { get; set; }

    public virtual DbSet<CompanyMaster> CompanyMasters { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<EmailRecipient> EmailRecipients { get; set; }

    public virtual DbSet<EmailSetting> EmailSettings { get; set; }

    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Lookup> Lookups { get; set; }

    public virtual DbSet<LookupType> LookupTypes { get; set; }
    public virtual DbSet<MailTemplate> MailTemplates { get; set; }

    public virtual DbSet<ModuleMaster> ModuleMasters { get; set; }

    public virtual DbSet<ProjectMaster> ProjectMasters { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    // Kept as alias for backward compatibility with repositories
    public virtual DbSet<RoleMasterSoftwareModule> RoleSoftwareModules { get; set; }
    public virtual DbSet<RoleMasterSoftwareModule> RoleMasterSoftwareModules { get; set; }

    // Kept as alias for backward compatibility with repositories
    public virtual DbSet<SoftwareModulesMaster> SoftwareModules { get; set; }
    public virtual DbSet<SoftwareModulesMaster> SoftwareModulesMasters { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<StatusMaster> StatusMasters { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkReport> WorkReports { get; set; }

    public virtual DbSet<WorkEntry> WorkEntries { get; set; }

    public virtual DbSet<WorkTimeLog> WorkTimeLogs { get; set; }
    public virtual DbSet<WorkLog> WorkLogs { get; set; }
    public virtual DbSet<WorkLogTask> WorkLogTasks { get; set; }
    public virtual DbSet<PhoneCallLog> PhoneCallLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkReport>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<WorkEntry>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
            entity.HasOne(d => d.WorkReport).WithMany(p => p.WorkEntries).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkTimeLog>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
            entity.HasOne(d => d.WorkEntry).WithMany(p => p.TimeLogs).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ClientMaster>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Company).WithMany(p => p.ClientMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientMaster_CompanyMaster");
        });

        modelBuilder.Entity<CompanyMaster>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasFillFactor(80);

            entity.Property(e => e.CompanyId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyMasters)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyMaster_CountryMaster");

            entity.HasOne(d => d.State).WithMany(p => p.CompanyMasters).HasConstraintName("FK_CompanyMaster_StateMaster");
        });

        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.Property(e => e.CountryId).ValueGeneratedNever();
        });

        modelBuilder.Entity<EmailRecipient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailRec__3214EC07E619AE4F");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmailRecipients).HasConstraintName("FK_EmailRecipients_EmployeeMaster");
        });

        modelBuilder.Entity<EmailSetting>(entity =>
        {
            entity.HasKey(e => e.EmailSettingsId).HasName("PK__EmailSet__24746D17D68426E7");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmailSettings).HasConstraintName("FK_EmailSettings_EmployeeMaster");
        });

        modelBuilder.Entity<EmployeeMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeId)
                .HasName("PK_StaffMaster")
                .HasFillFactor(80);

            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EmployeeName).HasComputedColumnSql("(concat_ws(' ',nullif(ltrim(rtrim([FirstName])),''),nullif(ltrim(rtrim([MiddleName])),''),nullif(ltrim(rtrim([LastName])),'')))", false);
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsAllowLogin).HasDefaultValue((byte)1);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Gender).WithMany(p => p.EmployeeMasters).HasConstraintName("FK_EmployeeMaster_Lookup");

            entity.HasOne(d => d.RoleMaster).WithMany(p => p.EmployeeMasters).HasConstraintName("FK_EmployeeMaster_RoleMaster");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ErrorLog__3214EC0788BF7A23");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Table_1");

            entity.Property(e => e.ActiveStatus).HasDefaultValue(true);

            entity.HasOne(d => d.Type).WithMany(p => p.Lookups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lookup_LookupType");
        });

        modelBuilder.Entity<LookupType>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue(true);
        });

        modelBuilder.Entity<ModuleMaster>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Company).WithMany(p => p.ModuleMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModuleMaster_CompanyMaster");

            entity.HasOne(d => d.ParentModule).WithMany(p => p.InverseParentModule).HasConstraintName("FK_ModuleMaster_ParentModule");
        });

        modelBuilder.Entity<ProjectMaster>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Company).WithMany(p => p.ProjectMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMaster_CompanyMaster");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasOne(d => d.Company).WithMany(p => p.RoleMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleMaster_InstituteMaster");

            entity.HasOne(d => d.RoleType).WithMany(p => p.RoleMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleMaster_Lookup");
        });

        modelBuilder.Entity<RoleMasterSoftwareModule>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.RoleMaster).WithMany(p => p.RoleMasterSoftwareModules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleMasterSoftwareModules_RoleMaster");

            entity.HasOne(d => d.SoftwareModulesMaster).WithMany(p => p.RoleMasterSoftwareModules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleMasterSoftwareModules_SoftwareModulesMaster");
        });

        modelBuilder.Entity<SoftwareModulesMaster>(entity =>
        {
            entity.HasKey(e => e.SoftwareModulesMasterId)
                .HasName("PK_ModulesMaster")
                .IsClustered(false)
                .HasFillFactor(80);

            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CrDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_SoftwareModulesMaster_SoftwareModulesMaster");
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.Property(e => e.StateId).ValueGeneratedNever();
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Company).WithMany(p => p.StatusMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusMaster_CompanyMaster");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MailTemplate>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<WorkLog>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<WorkLogTask>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
            entity.HasOne(d => d.WorkLog).WithMany(p => p.WorkLogTasks).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PhoneCallLog>(entity =>
        {
            entity.Property(e => e.ActiveStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Guids).HasDefaultValueSql("(newid())");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
