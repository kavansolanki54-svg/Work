using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Extensions;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Contact Detail page — per-contact analytics drill-down.
    /// </summary>
    [QueryProperty(nameof(PhoneNumber), "phoneNumber")]
    public partial class ContactDetailViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILoggerService _logger;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private ContactAnalyticsModel? _contactAnalytics;

        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string _initials = string.Empty;

        public ContactDetailViewModel(IAnalyticsService analyticsService, ILoggerService logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
            Title = "Contact Detail";
        }

        partial void OnPhoneNumberChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _ = LoadContactAsync();
            }
        }

        [RelayCommand]
        private async Task LoadContactAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var decoded = Uri.UnescapeDataString(PhoneNumber);
                var now = DateTime.UtcNow;
                var from = now.AddMonths(-3); // Last 3 months by default

                ContactAnalytics = await _analyticsService.GetContactAnalyticsAsync(decoded, from, now);

                if (ContactAnalytics != null)
                {
                    DisplayName = !string.IsNullOrEmpty(ContactAnalytics.ContactName)
                        ? ContactAnalytics.ContactName
                        : Helpers.PhoneNumberHelper.FormatForDisplay(decoded);
                    Initials = Helpers.PhoneNumberHelper.GetInitials(
                        !string.IsNullOrEmpty(ContactAnalytics.ContactName)
                            ? ContactAnalytics.ContactName
                            : decoded);
                }

                _logger.Info("ContactDetailVM", $"Loaded analytics for {decoded.MaskPhoneNumber()}");
            }, "Failed to load contact analytics");
        }

        [RelayCommand]
        private async Task CompareAsync()
        {
            if (string.IsNullOrEmpty(PhoneNumber)) return;

            // In a real app, we'd open a contact picker here to choose the second contact.
            // For now, we'll navigate to the CompareContactsPage and it can handle selecting contact2.
            var encoded = Uri.EscapeDataString(PhoneNumber);
            await Shell.Current.GoToAsync($"comparecontacts?contact1={encoded}");
        }
    }
}
