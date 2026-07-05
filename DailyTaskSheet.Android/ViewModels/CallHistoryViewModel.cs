using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Call History screen.
    /// Provides paginated call log records with pull-to-refresh support.
    /// </summary>
    public class CallHistoryViewModel : BaseViewModel
    {
        private readonly ICallLogRepository _callLogRepo;
        private readonly ILoggerService _logger;
        private const int PageSize = 50;
        private int _currentOffset;

        private List<CallLogModel> _callLogs = new List<CallLogModel>();
        private bool _hasMore = true;

        /// <summary>Gets the current list of call log records.</summary>
        public List<CallLogModel> CallLogs
        {
            get => _callLogs;
            set => SetProperty(ref _callLogs, value);
        }

        /// <summary>Gets whether there are more records to load.</summary>
        public bool HasMore
        {
            get => _hasMore;
            set => SetProperty(ref _hasMore, value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CallHistoryViewModel"/>.
        /// </summary>
        public CallHistoryViewModel(ICallLogRepository callLogRepo, ILoggerService logger)
        {
            _callLogRepo = callLogRepo;
            _logger = logger;
            Title = "Call History";
        }

        /// <summary>
        /// Loads the initial page of call logs.
        /// </summary>
        public async Task LoadAsync()
        {
            IsBusy = true;
            _currentOffset = 0;
            try
            {
                var records = await _callLogRepo.GetRecentAsync(PageSize, 0);
                CallLogs = records;
                HasMore = records.Count >= PageSize;
                _currentOffset = records.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("CallHistoryVM", "Failed to load call logs", ex);
                ErrorMessage = "Failed to load call history.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Loads the next page of call logs (infinite scroll).
        /// </summary>
        public async Task LoadMoreAsync()
        {
            if (!HasMore || IsBusy) return;

            try
            {
                var records = await _callLogRepo.GetRecentAsync(PageSize, _currentOffset);
                if (records.Count > 0)
                {
                    _callLogs.AddRange(records);
                    OnPropertyChanged(nameof(CallLogs));
                    _currentOffset += records.Count;
                }
                HasMore = records.Count >= PageSize;
            }
            catch (Exception ex)
            {
                _logger.Error("CallHistoryVM", "Failed to load more call logs", ex);
            }
        }

        /// <summary>
        /// Refreshes the call log list from the beginning (pull-to-refresh).
        /// </summary>
        public async Task RefreshAsync()
        {
            await LoadAsync();
        }
    }
}
