using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CsvHelper;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private int GetEmployeeId()
        {
            var empIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (empIdClaim != null && int.TryParse(empIdClaim.Value, out int empId))
            {
                return empId;
            }
            throw new UnauthorizedAccessException("Employee ID not found in token.");
        }

        [HttpGet("csv")]
        public async Task<IActionResult> ExportToCsv([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var employeeId = GetEmployeeId();
                var query = _context.PhoneCallLogs.Where(l => l.EmployeeId == employeeId && l.ActiveStatus == 1);
                
                if (from.HasValue) query = query.Where(l => l.StartTime >= from.Value);
                if (to.HasValue) query = query.Where(l => l.StartTime <= to.Value);

                var logs = await query.OrderByDescending(l => l.StartTime).ToListAsync();

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                var exportData = logs.Select(l => new {
                    l.PhoneNumber,
                    ContactName = l.ContactName ?? "Unknown",
                    l.CallType,
                    StartTime = l.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    DurationSeconds = l.DurationInSeconds
                });

                await csv.WriteRecordsAsync(exportData);
                await writer.FlushAsync();
                
                return File(memoryStream.ToArray(), "text/csv", $"CallLogs_Export_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error exporting CSV: {ex.Message}"));
            }
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> ExportToPdf([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var employeeId = GetEmployeeId();
                var query = _context.PhoneCallLogs.Where(l => l.EmployeeId == employeeId && l.ActiveStatus == 1);
                
                if (from.HasValue) query = query.Where(l => l.StartTime >= from.Value);
                if (to.HasValue) query = query.Where(l => l.StartTime <= to.Value);

                var logs = await query.OrderByDescending(l => l.StartTime).Take(500).ToListAsync(); // Limit to 500 for PDF to avoid massive files

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(x => ComposeContent(x, logs));
                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"CallLogs_Report_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.ErrorResponse($"Error exporting PDF: {ex.Message}"));
            }
        }

        private void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Call Analytics Report").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text($"Generated on {DateTime.Now:MMMM dd, yyyy}").FontSize(12).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container, List<PhoneCallLog> logs)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Number/Name
                        columns.RelativeColumn(2); // Type
                        columns.RelativeColumn(3); // Date
                        columns.RelativeColumn(2); // Duration
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Contact").SemiBold();
                        header.Cell().Text("Call Type").SemiBold();
                        header.Cell().Text("Date & Time").SemiBold();
                        header.Cell().Text("Duration").SemiBold();
                    });

                    foreach (var log in logs)
                    {
                        table.Cell().Text($"{log.PhoneNumber}\n{(log.ContactName ?? "Unknown")}");
                        table.Cell().Text(log.CallType);
                        table.Cell().Text(log.StartTime.ToString("g"));
                        table.Cell().Text($"{log.DurationInSeconds}s");
                    }
                });
            });
        }
    }
}
