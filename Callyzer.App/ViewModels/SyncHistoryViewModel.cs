using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.Repositories;

namespace Callyzer.App.ViewModels
{
    public partial class SyncHistoryViewModel : BaseViewModel
    {
        private readonly CallLogRepository _callLogRepo;
        private readonly ISyncService _syncService;

        [ObservableProperty]
        private ObservableCollection<CallLogModel> _syncedLogs = new();

        [ObservableProperty]
        private bool _isSyncing;

        [ObservableProperty]
        private int _pendingSyncCount;

        [ObservableProperty]
        private string _lastSyncMessage = "Ready";

        public SyncHistoryViewModel(CallLogRepository callLogRepo, ISyncService syncService)
        {
            _callLogRepo = callLogRepo;
            _syncService = syncService;
            Title = "Sync History";

            _syncService.SyncProgressChanged += OnSyncProgressChanged;
        }

        private void OnSyncProgressChanged(object? sender, SyncProgressEventArgs e)
        {
            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
            {
                IsSyncing = e.IsSyncing;
                LastSyncMessage = e.Message;
                if (!e.IsSyncing)
                {
                    LoadHistoryCommand.Execute(null);
                }
            });
        }

        [RelayCommand]
        private async Task LoadHistoryAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                PendingSyncCount = await _syncService.GetPendingSyncCountAsync();

                // Get the last 100 logs
                // Note: GetLogsAsync returns all logs. We'll filter for synced in memory, 
                // or ideally add a GetSyncedLogsAsync to the repo later.
                var logs = await _callLogRepo.GetLogsAsync(0, 100);
                
                SyncedLogs.Clear();
                foreach (var log in logs)
                {
                    SyncedLogs.Add(log);
                }
            }, "Failed to load sync history");
        }

        [RelayCommand]
        private async Task TriggerSyncAsync()
        {
            if (IsSyncing) return;
            await _syncService.SyncPendingLogsAsync(isBackground: false);
        }
    }
}
