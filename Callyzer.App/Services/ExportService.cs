using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestContainer = QuestPDF.Infrastructure.IContainer;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.Repositories;

namespace Callyzer.App.Services
{
    public class ExportService : IExportService
    {
        private readonly CallLogRepository _callLogRepo;
        private readonly ILoggerService _logger;
        private readonly string _exportDirectory;

        public ExportService(CallLogRepository callLogRepo, ILoggerService logger)
        {
            _callLogRepo = callLogRepo;
            _logger = logger;
            _exportDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Exports");
            
            if (!Directory.Exists(_exportDirectory))
            {
                Directory.CreateDirectory(_exportDirectory);
            }

            // Set QuestPDF Community License
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> ExportToCsvAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            try
            {
                var logs = await _callLogRepo.GetLogsByDateRangeAsync(from, to, ct);
                var filePath = Path.Combine(_exportDirectory, $"Callyzer_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csv.WriteRecordsAsync(logs, ct);
                }

                _logger.Info("ExportService", $"CSV export completed: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error("ExportService", "Failed to export CSV", ex);
                throw;
            }
        }

        public async Task<string> ExportToPdfAsync(AnalyticsSummaryModel summary, List<ContactAnalyticsModel> topContacts, CancellationToken ct = default)
        {
            try
            {
                var filePath = Path.Combine(_exportDirectory, $"Callyzer_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(QuestPDF.Helpers.Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(x => ComposeContent(x, summary, topContacts));
                        page.Footer().Element(ComposeFooter);
                    });
                });

                document.GeneratePdf(filePath);
                _logger.Info("ExportService", $"PDF export completed: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error("ExportService", "Failed to export PDF", ex);
                throw;
            }
        }

        public async Task ShareFileAsync(string filePath, string title)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.Warning("ExportService", "Attempted to share non-existent file.");
                    return;
                }

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                _logger.Error("ExportService", "Failed to share file", ex);
            }
        }

        private void ComposeHeader(QuestContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Call Analytics Report").Style(titleStyle);
                    column.Item().Text(text =>
                    {
                        text.Span("Generated: ").SemiBold();
                        text.Span($"{DateTime.Now:g}");
                    });
                });
            });
        }

        void ComposeContent(QuestContainer container, AnalyticsSummaryModel summary, List<ContactAnalyticsModel> topContacts)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(20);

                column.Item().Element(c => ComposeSummary(c, summary));
                column.Item().Element(c => ComposeTopContacts(c, topContacts));
            });
        }

        void ComposeSummary(QuestContainer container, AnalyticsSummaryModel summary)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("Total Calls").SemiBold();
                    header.Cell().Text("Total Duration").SemiBold();
                    header.Cell().Text("Unique Contacts").SemiBold();
                });

                table.Cell().Text(summary.TotalCalls.ToString());
                table.Cell().Text($"{summary.TotalDuration / 60} min");
                table.Cell().Text(summary.UniqueContacts.ToString());
            });
        }

        void ComposeTopContacts(QuestContainer container, List<ContactAnalyticsModel> topContacts)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(5).Text("Top Contacts").FontSize(14).SemiBold();
                
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("#").SemiBold();
                        header.Cell().Text("Name").SemiBold();
                        header.Cell().Text("Phone").SemiBold();
                        header.Cell().Text("Calls").SemiBold();
                    });

                    int i = 1;
                    foreach (var contact in topContacts)
                    {
                        table.Cell().Text(i.ToString());
                        table.Cell().Text(contact.ContactName ?? "Unknown");
                        table.Cell().Text(contact.PhoneNumber);
                        table.Cell().Text(contact.TotalCalls.ToString());
                        i++;
                    }
                });
            });
        }

        private void ComposeFooter(QuestContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
                x.Span(" of ");
                x.TotalPages();
            });
        }
    }
}
