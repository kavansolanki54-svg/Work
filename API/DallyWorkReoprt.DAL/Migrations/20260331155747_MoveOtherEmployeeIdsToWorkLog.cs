using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DallyWorkReoprt.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MoveOtherEmployeeIdsToWorkLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryMaster",
                columns: table => new
                {
                    CountryID = table.Column<int>(type: "int", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CurrencySymbole = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryMaster", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ErrorLog__3214EC0788BF7A23", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookupType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectFormat = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeaderHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FooterHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedByID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifiedBYID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TableConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    JwtId = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareModulesMaster",
                columns: table => new
                {
                    SoftwareModulesMasterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ModulesName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    ControllersName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    ActionName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    HasCreate = table.Column<bool>(type: "bit", nullable: true),
                    FullURL = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CrDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsNew = table.Column<bool>(type: "bit", nullable: true),
                    ExternalURI = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    DisplayLevel = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulesMaster", x => x.SoftwareModulesMasterID)
                        .Annotation("SqlServer:Clustered", false)
                        .Annotation("SqlServer:FillFactor", 80);
                    table.ForeignKey(
                        name: "FK_SoftwareModulesMaster_SoftwareModulesMaster",
                        column: x => x.ParentId,
                        principalTable: "SoftwareModulesMaster",
                        principalColumn: "SoftwareModulesMasterID");
                });

            migrationBuilder.CreateTable(
                name: "StateMaster",
                columns: table => new
                {
                    StateID = table.Column<int>(type: "int", nullable: false),
                    CountryID = table.Column<int>(type: "int", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMaster", x => x.StateID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<short>(type: "smallint", nullable: false),
                    LookupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table_1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lookup_LookupType",
                        column: x => x.TypeId,
                        principalTable: "LookupType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyMaster",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    IsEmailVerified = table.Column<byte>(type: "tinyint", nullable: false),
                    PhoneNo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IsMobileNoVerified = table.Column<byte>(type: "tinyint", nullable: false),
                    Website = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    PreferredSubDomain = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    FullAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CountryID = table.Column<int>(type: "int", nullable: true),
                    StateID = table.Column<int>(type: "int", nullable: true),
                    CityName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Pincode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogoUrl = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyMaster", x => x.CompanyID)
                        .Annotation("SqlServer:FillFactor", 80);
                    table.ForeignKey(
                        name: "FK_CompanyMaster_CountryMaster",
                        column: x => x.CompanyID,
                        principalTable: "CountryMaster",
                        principalColumn: "CountryID");
                    table.ForeignKey(
                        name: "FK_CompanyMaster_StateMaster",
                        column: x => x.StateID,
                        principalTable: "StateMaster",
                        principalColumn: "StateID");
                });

            migrationBuilder.CreateTable(
                name: "ClientMaster",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientShortCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMaster", x => x.ClientID);
                    table.ForeignKey(
                        name: "FK_ClientMaster_CompanyMaster",
                        column: x => x.CompanyID,
                        principalTable: "CompanyMaster",
                        principalColumn: "CompanyID");
                });

            migrationBuilder.CreateTable(
                name: "ModuleMaster",
                columns: table => new
                {
                    ModuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ParentModuleID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleMaster", x => x.ModuleID);
                    table.ForeignKey(
                        name: "FK_ModuleMaster_CompanyMaster",
                        column: x => x.CompanyID,
                        principalTable: "CompanyMaster",
                        principalColumn: "CompanyID");
                    table.ForeignKey(
                        name: "FK_ModuleMaster_ParentModule",
                        column: x => x.ParentModuleID,
                        principalTable: "ModuleMaster",
                        principalColumn: "ModuleID");
                });

            migrationBuilder.CreateTable(
                name: "ProjectMaster",
                columns: table => new
                {
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProjectColor = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMaster", x => x.ProjectID);
                    table.ForeignKey(
                        name: "FK_ProjectMaster_CompanyMaster",
                        column: x => x.CompanyID,
                        principalTable: "CompanyMaster",
                        principalColumn: "CompanyID");
                });

            migrationBuilder.CreateTable(
                name: "RoleMaster",
                columns: table => new
                {
                    RoleMasterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleTypeId = table.Column<int>(type: "int", nullable: false),
                    Descriptions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMaster", x => x.RoleMasterId);
                    table.ForeignKey(
                        name: "FK_RoleMaster_InstituteMaster",
                        column: x => x.CompanyId,
                        principalTable: "CompanyMaster",
                        principalColumn: "CompanyID");
                    table.ForeignKey(
                        name: "FK_RoleMaster_Lookup",
                        column: x => x.RoleTypeId,
                        principalTable: "Lookup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StatusMaster",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StatusColor = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusMaster", x => x.StatusID);
                    table.ForeignKey(
                        name: "FK_StatusMaster_CompanyMaster",
                        column: x => x.CompanyID,
                        principalTable: "CompanyMaster",
                        principalColumn: "CompanyID");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeMaster",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    RoleMasterID = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Passwords = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    MobileNo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    EmployeePhotoFile = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsAllowLogin = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    EmployeeCode = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DOJ = table.Column<DateOnly>(type: "date", nullable: true),
                    GenderID = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignInAttempt = table.Column<byte>(type: "tinyint", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(302)", maxLength: 302, nullable: false, computedColumnSql: "(concat_ws(' ',nullif(ltrim(rtrim([FirstName])),''),nullif(ltrim(rtrim([MiddleName])),''),nullif(ltrim(rtrim([LastName])),'')))", stored: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Tenant = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMaster", x => x.EmployeeID)
                        .Annotation("SqlServer:FillFactor", 80);
                    table.ForeignKey(
                        name: "FK_EmployeeMaster_Lookup",
                        column: x => x.GenderID,
                        principalTable: "Lookup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeMaster_RoleMaster",
                        column: x => x.RoleMasterID,
                        principalTable: "RoleMaster",
                        principalColumn: "RoleMasterId");
                });

            migrationBuilder.CreateTable(
                name: "RoleMasterSoftwareModules",
                columns: table => new
                {
                    RoleMasterSoftwareModulesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleMasterID = table.Column<int>(type: "int", nullable: false),
                    SoftwareModulesMasterID = table.Column<int>(type: "int", nullable: false),
                    View = table.Column<bool>(type: "bit", nullable: true),
                    Add = table.Column<bool>(type: "bit", nullable: true),
                    Edit = table.Column<bool>(type: "bit", nullable: true),
                    Delete = table.Column<bool>(type: "bit", nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedByID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMasterSoftwareModules", x => x.RoleMasterSoftwareModulesID);
                    table.ForeignKey(
                        name: "FK_RoleMasterSoftwareModules_RoleMaster",
                        column: x => x.RoleMasterID,
                        principalTable: "RoleMaster",
                        principalColumn: "RoleMasterId");
                    table.ForeignKey(
                        name: "FK_RoleMasterSoftwareModules_SoftwareModulesMaster",
                        column: x => x.SoftwareModulesMasterID,
                        principalTable: "SoftwareModulesMaster",
                        principalColumn: "SoftwareModulesMasterID");
                });

            migrationBuilder.CreateTable(
                name: "EmailRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecipientType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailRec__3214EC07E619AE4F", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailRecipients_EmployeeMaster",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMaster",
                        principalColumn: "EmployeeID");
                });

            migrationBuilder.CreateTable(
                name: "EmailSettings",
                columns: table => new
                {
                    EmailSettingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmtpServer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ActiveStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailSet__24746D17D68426E7", x => x.EmailSettingsId);
                    table.ForeignKey(
                        name: "FK_EmailSettings_EmployeeMaster",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeMaster",
                        principalColumn: "EmployeeID");
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    WorkLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InputTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDuration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalMinutes = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifiedBYID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    OtherEmployeeIds = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.WorkLogId);
                    table.ForeignKey(
                        name: "FK_WorkLogs_ClientMaster_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientMaster",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogs_EmployeeMaster_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMaster",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogs_ProjectMaster_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectMaster",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedByID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBYID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkReports_EmployeeMaster_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeMaster",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogTasks",
                columns: table => new
                {
                    WorkLogTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedByID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogTasks", x => x.WorkLogTaskId);
                    table.ForeignKey(
                        name: "FK_WorkLogTasks_StatusMaster_StatusId",
                        column: x => x.StatusId,
                        principalTable: "StatusMaster",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogTasks_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "WorkLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkReportId = table.Column<int>(type: "int", nullable: false),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedByID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBYID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkEntries_ProjectMaster_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectMaster",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkEntries_StatusMaster_StatusId",
                        column: x => x.StatusId,
                        principalTable: "StatusMaster",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkEntries_WorkReports_WorkReportId",
                        column: x => x.WorkReportId,
                        principalTable: "WorkReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkTimeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkEntryId = table.Column<int>(type: "int", nullable: false),
                    InTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OutTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Is30MinBreak = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreatedByID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBYID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTimeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTimeLogs_WorkEntries_WorkEntryId",
                        column: x => x.WorkEntryId,
                        principalTable: "WorkEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMaster_CompanyID",
                table: "ClientMaster",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMaster_StateID",
                table: "CompanyMaster",
                column: "StateID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_EmployeeID",
                table: "EmailRecipients",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSettings_EmployeeID",
                table: "EmailSettings",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMaster_GenderID",
                table: "EmployeeMaster",
                column: "GenderID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMaster_RoleMasterID",
                table: "EmployeeMaster",
                column: "RoleMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_Lookup_TypeId",
                table: "Lookup",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleMaster_CompanyID",
                table: "ModuleMaster",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleMaster_ParentModuleID",
                table: "ModuleMaster",
                column: "ParentModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMaster_CompanyID",
                table: "ProjectMaster",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_CompanyId",
                table: "RoleMaster",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMaster_RoleTypeId",
                table: "RoleMaster",
                column: "RoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMasterSoftwareModules_RoleMasterID",
                table: "RoleMasterSoftwareModules",
                column: "RoleMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMasterSoftwareModules_SoftwareModulesMasterID",
                table: "RoleMasterSoftwareModules",
                column: "SoftwareModulesMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareModulesMaster_ParentId",
                table: "SoftwareModulesMaster",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusMaster_CompanyID",
                table: "StatusMaster",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkEntries_ProjectId",
                table: "WorkEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkEntries_StatusId",
                table: "WorkEntries",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkEntries_WorkReportId",
                table: "WorkEntries",
                column: "WorkReportId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ClientId",
                table: "WorkLogs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_EmployeeId",
                table: "WorkLogs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogTasks_StatusId",
                table: "WorkLogTasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogTasks_WorkLogId",
                table: "WorkLogTasks",
                column: "WorkLogId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkReports_EmployeeId",
                table: "WorkReports",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimeLogs_WorkEntryId",
                table: "WorkTimeLogs",
                column: "WorkEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailRecipients");

            migrationBuilder.DropTable(
                name: "EmailSettings");

            migrationBuilder.DropTable(
                name: "ErrorLog");

            migrationBuilder.DropTable(
                name: "MailTemplates");

            migrationBuilder.DropTable(
                name: "ModuleMaster");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RoleMasterSoftwareModules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkLogTasks");

            migrationBuilder.DropTable(
                name: "WorkTimeLogs");

            migrationBuilder.DropTable(
                name: "SoftwareModulesMaster");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "WorkEntries");

            migrationBuilder.DropTable(
                name: "ClientMaster");

            migrationBuilder.DropTable(
                name: "ProjectMaster");

            migrationBuilder.DropTable(
                name: "StatusMaster");

            migrationBuilder.DropTable(
                name: "WorkReports");

            migrationBuilder.DropTable(
                name: "EmployeeMaster");

            migrationBuilder.DropTable(
                name: "RoleMaster");

            migrationBuilder.DropTable(
                name: "CompanyMaster");

            migrationBuilder.DropTable(
                name: "Lookup");

            migrationBuilder.DropTable(
                name: "CountryMaster");

            migrationBuilder.DropTable(
                name: "StateMaster");

            migrationBuilder.DropTable(
                name: "LookupType");
        }
    }
}
