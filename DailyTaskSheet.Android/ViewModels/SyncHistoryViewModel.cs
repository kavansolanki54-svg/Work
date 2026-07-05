using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.ViewModels
{
    /// <summary>
    /// ViewModel for the Sync History screen.
    /// Displays historical sync operations with their outcomes.
    /// </summary>
    public class SyncHistoryViewModel : BaseViewModel
    {
        private readonly ISyncRepository _syncRepo;
        private readonly ILoggerService _logger;
        private const int MaxHistoryRecords = 100;

        private List<SyncHistoryModel> _syncHistory = new List<SyncHistoryModel>();

        /// <summary>Gets the sync history records.</summary>
        public List<SyncHistoryModel> SyncHistory
        {
            get => _syncHistory;
            set => SetProperty(ref _syncHistory, value);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SyncHistoryViewModel"/>.
        /// </summary>
        public SyncHistoryViewModel(ISyncRepository syncRepo, ILoggerService logger)
        {
            _syncRepo = syncRepo;
            _logger = logger;
            Title = "Sync History";
        }

        /// <summary>
        /// Loads sync history records from the database.
        /// </summary>
        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                SyncHistory = await _syncRepo.GetRecentHistoryAsync(MaxHistoryRecords);
            }
            catch (Exception ex)
            {
                _logger.Error("SyncHistoryVM", "Failed to load sync history", ex);
                ErrorMessage = "Failed to load sync history.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
