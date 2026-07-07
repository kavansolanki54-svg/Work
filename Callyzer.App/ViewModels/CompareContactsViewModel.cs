using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.ViewModels
{
    [QueryProperty(nameof(Contact1), "contact1")]
    [QueryProperty(nameof(Contact2), "contact2")]
    public partial class CompareContactsViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        private string _contact1 = string.Empty;

        [ObservableProperty]
        private string _contact2 = string.Empty;

        [ObservableProperty]
        private string _contact1Name = string.Empty;

        [ObservableProperty]
        private string _contact2Name = string.Empty;

        [ObservableProperty]
        private string _contact2Input = string.Empty;

        [ObservableProperty]
        private ComparisonResultModel? _result;

        [ObservableProperty]
        private Chart? _callsChart;

        [ObservableProperty]
        private Chart? _durationChart;

        public bool IsContact2Empty => string.IsNullOrEmpty(Contact2);
        public bool HasResults => Result != null && Result.Contacts.Count == 2;

        public CompareContactsViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            Title = "Compare Contacts";
        }

        partial void OnContact1Changed(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Contact1Name = Helpers.PhoneNumberHelper.FormatForDisplay(Uri.UnescapeDataString(value));
            }
        }

        partial void OnContact2Changed(string value)
        {
            OnPropertyChanged(nameof(IsContact2Empty));
            if (!string.IsNullOrEmpty(value))
            {
                Contact2Name = Helpers.PhoneNumberHelper.FormatForDisplay(Uri.UnescapeDataString(value));
                _ = LoadComparisonAsync();
            }
            else
            {
                Result = null;
                OnPropertyChanged(nameof(HasResults));
            }
        }

        [RelayCommand]
        private void Compare()
        {
            if (!string.IsNullOrWhiteSpace(Contact2Input))
            {
                Contact2 = Contact2Input; // This triggers OnContact2Changed -> LoadComparisonAsync
            }
        }

        private async Task LoadComparisonAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var decoded1 = Uri.UnescapeDataString(Contact1);
                var decoded2 = Uri.UnescapeDataString(Contact2);

                var now = DateTime.UtcNow;
                var from = now.AddMonths(-3); // Last 3 months default

                Result = await _analyticsService.CompareContactsAsync(new[] { decoded1, decoded2 }, from, now);
                
                if (Result != null && Result.Contacts.Count == 2)
                {
                    // If we got real names from the DB, update the display names
                    if (!string.IsNullOrEmpty(Result.Contacts[0].ContactName)) Contact1Name = Result.Contacts[0].ContactName;
                    if (!string.IsNullOrEmpty(Result.Contacts[1].ContactName)) Contact2Name = Result.Contacts[1].ContactName;

                    GenerateCharts();
                    OnPropertyChanged(nameof(HasResults));
                }
            }, "Failed to compare contacts");
        }

        private void GenerateCharts()
        {
            if (Result == null || Result.Contacts.Count < 2) return;

            var c1 = Result.Contacts[0];
            var c2 = Result.Contacts[1];

            var color1 = SKColor.Parse("#3498db"); // Blue
            var color2 = SKColor.Parse("#e74c3c"); // Red

            // Total Calls Bar Chart
            CallsChart = new BarChart
            {
                Entries = new[]
                {
                    new ChartEntry(c1.TotalCalls) { Label = Contact1Name, ValueLabel = c1.TotalCalls.ToString(), Color = color1 },
                    new ChartEntry(c2.TotalCalls) { Label = Contact2Name, ValueLabel = c2.TotalCalls.ToString(), Color = color2 }
                },
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 30f,
                ValueLabelOrientation = Orientation.Horizontal
            };

            // Total Duration Bar Chart
            DurationChart = new BarChart
            {
                Entries = new[]
                {
                    new ChartEntry(c1.TotalDuration) { Label = Contact1Name, ValueLabel = $"{c1.TotalDuration / 60}m", Color = color1 },
                    new ChartEntry(c2.TotalDuration) { Label = Contact2Name, ValueLabel = $"{c2.TotalDuration / 60}m", Color = color2 }
                },
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 30f,
                ValueLabelOrientation = Orientation.Horizontal
            };
        }
    }
}
