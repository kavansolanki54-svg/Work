using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DallyWorkReoprt.DAL.Migrations
{
    public partial class ChangeWorkLogToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('WorkLogTasks') IS NOT NULL DROP TABLE WorkLogTasks;");
            migrationBuilder.Sql("IF OBJECT_ID('WorkLogs') IS NOT NULL DROP TABLE WorkLogs;");

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    WorkLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InputTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDuration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalMinutes = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Guids = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    OtherEmployeeIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_WorkLogs_CompanyMaster_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyMaster",
                        principalColumn: "CompanyID",
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
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_WorkLogs_StatusMaster_StatusId",
                        column: x => x.StatusId,
                        principalTable: "StatusMaster",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogTasks",
                columns: table => new
                {
                    WorkLogTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkLogId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ClientId",
                table: "WorkLogs",
                column: "ClientId");
            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_CompanyId",
                table: "WorkLogs",
                column: "CompanyId");
            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_EmployeeId",
                table: "WorkLogs",
                column: "EmployeeId");
            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");
            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_StatusId",
                table: "WorkLogs",
                column: "StatusId");
            migrationBuilder.CreateIndex(
                name: "IX_WorkLogTasks_StatusId",
                table: "WorkLogTasks",
                column: "StatusId");
            migrationBuilder.CreateIndex(
                name: "IX_WorkLogTasks_WorkLogId",
                table: "WorkLogTasks",
                column: "WorkLogId");
            
            // Adding changes that EF also wanted
            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "WorkEntries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkEntries_ModuleId",
                table: "WorkEntries",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkEntries_ModuleMaster_ModuleId",
                table: "WorkEntries",
                column: "ModuleId",
                principalTable: "ModuleMaster",
                principalColumn: "ModuleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("WorkLogTasks");
            migrationBuilder.DropTable("WorkLogs");
            
            migrationBuilder.DropForeignKey(
                name: "FK_WorkEntries_ModuleMaster_ModuleId",
                table: "WorkEntries");

            migrationBuilder.DropIndex(
                name: "IX_WorkEntries_ModuleId",
                table: "WorkEntries");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "WorkEntries");
        }
    }
}
