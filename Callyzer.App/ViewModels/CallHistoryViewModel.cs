using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Enums;
using Callyzer.App.Extensions;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Call History page with search, filter, and pagination.
    /// </summary>
    public partial class CallHistoryViewModel : BaseViewModel
    {
        private readonly IRepository<CallLogModel> _callLogRepo;
        private readonly ILoggerService _logger;
        private const int PageSize = 50;
        private int _currentPage = 0;

        [ObservableProperty]
        private ObservableCollection<CallLogModel> _callLogs = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private CallTypeEnum? _filterCallType;

        [ObservableProperty]
        private bool _hasMoreData = true;

        [ObservableProperty]
        private int _totalCount;

        public CallHistoryViewModel(IRepository<CallLogModel> callLogRepo, ILoggerService logger)
        {
            _callLogRepo = callLogRepo;
            _logger = logger;
            Title = "Call History";
        }

        [RelayCommand]
        private async Task LoadCallLogsAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                _currentPage = 0;
                CallLogs.Clear();
                TotalCount = await _callLogRepo.CountAsync();
                await LoadNextPageAsync();
            }, "Failed to load call history");
        }

        [RelayCommand]
        private async Task LoadNextPageAsync()
        {
            if (!HasMoreData || IsBusy) return;

            try
            {
                IsBusy = true;
                var offset = _currentPage * PageSize;

                List<CallLogModel> results;
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    var query = SearchQuery.Trim().ToLowerInvariant();
                    results = await _callLogRepo.FindAsync(c =>
                        c.PhoneNumber.Contains(query) ||
                        c.ContactName.ToLower().Contains(query));
                }
                else if (FilterCallType.HasValue)
                {
                    var typeInt = (int)FilterCallType.Value;
                    results = await _callLogRepo.FindAsync(c => c.CallType == typeInt);
                }
                else
                {
                    results = await _callLogRepo.GetAllAsync();
                }

                // Manual pagination since SQLite-net doesn't support Skip/Take in expressions
                var page = results
                    .OrderByDescending(c => c.CallDate)
                    .Skip(offset)
                    .Take(PageSize)
                    .ToList();

                foreach (var log in page)
                {
                    CallLogs.Add(log);
                }

                HasMoreData = page.Count == PageSize;
                _currentPage++;

                _logger.Debug("CallHistoryVM", $"Loaded page {_currentPage}, {page.Count} items");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            _currentPage = 0;
            CallLogs.Clear();
            HasMoreData = true;
            await LoadNextPageAsync();
        }

        [RelayCommand]
        private async Task FilterByTypeAsync(string? type)
        {
            if (string.IsNullOrEmpty(type) || type == "All")
            {
                FilterCallType = null;
            }
            else if (Enum.TryParse<CallTypeEnum>(type, out var parsed))
            {
                FilterCallType = parsed;
            }
            _currentPage = 0;
            CallLogs.Clear();
            HasMoreData = true;
            await LoadNextPageAsync();
        }

        [RelayCommand]
        private async Task NavigateToContactDetailAsync(CallLogModel callLog)
        {
            if (callLog == null) return;
            var encoded = Uri.EscapeDataString(callLog.PhoneNumber);
            await Shell.Current.GoToAsync($"contactdetail?phoneNumber={encoded}");
        }
    }
}
