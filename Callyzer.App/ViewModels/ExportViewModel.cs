using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Callyzer.App.Enums;
using Callyzer.App.Interfaces;

namespace Callyzer.App.ViewModels
{
    public partial class ExportViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        private string _selectedFormat = "CSV";

        [ObservableProperty]
        private int _selectedDays = 7;

        public Style CsvButtonStyle => SelectedFormat == "CSV" ? GetStyle("PrimaryButton") : GetStyle("SecondaryButton");
        public Style PdfButtonStyle => SelectedFormat == "PDF" ? GetStyle("PrimaryButton") : GetStyle("SecondaryButton");

        public Style WeekButtonStyle => SelectedDays == 7 ? GetStyle("PrimaryButton") : GetStyle("SecondaryButton");
        public Style MonthButtonStyle => SelectedDays == 30 ? GetStyle("PrimaryButton") : GetStyle("SecondaryButton");
        public Style AllTimeButtonStyle => SelectedDays == 0 ? GetStyle("PrimaryButton") : GetStyle("SecondaryButton");

        public ExportViewModel(IExportService exportService, IAnalyticsService analyticsService)
        {
            _exportService = exportService;
            _analyticsService = analyticsService;
            Title = "Export Data";
        }

        [RelayCommand]
        private void SelectFormat(string format)
        {
            SelectedFormat = format;
            OnPropertyChanged(nameof(CsvButtonStyle));
            OnPropertyChanged(nameof(PdfButtonStyle));
        }

        [RelayCommand]
        private void SelectRange(string daysStr)
        {
            if (int.TryParse(daysStr, out var days))
            {
                SelectedDays = days;
                OnPropertyChanged(nameof(WeekButtonStyle));
                OnPropertyChanged(nameof(MonthButtonStyle));
                OnPropertyChanged(nameof(AllTimeButtonStyle));
            }
        }

        [RelayCommand]
        private async Task ExportAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var to = DateTime.UtcNow;
                var from = SelectedDays == 0 ? DateTime.MinValue : to.AddDays(-SelectedDays);
                var title = $"Callyzer Export ({SelectedFormat})";
                string filePath;

                if (SelectedFormat == "CSV")
                {
                    filePath = await _exportService.ExportToCsvAsync(from, to);
                }
                else
                {
                    // Generate PDF
                    var summary = await _analyticsService.GetCustomRangeSummaryAsync(from, to);
                    var topContacts = await _analyticsService.GetTopContactsAsync(50, from, to);
                    filePath = await _exportService.ExportToPdfAsync(summary, topContacts);
                }

                await _exportService.ShareFileAsync(filePath, title);

            }, "Failed to generate export file.");
        }

        private Style GetStyle(string key)
        {
            if (Application.Current?.Resources.TryGetValue(key, out var style) == true)
            {
                return (Style)style;
            }
            return new Style(typeof(Button));
        }
    }
}
