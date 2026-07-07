using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.ViewModels
{
    public partial class BackupRestoreViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private readonly IExportService _exportService; // For sharing backup ZIPs
        private readonly ILoggerService _logger;

        [ObservableProperty]
        private ObservableCollection<BackupInfoModel> _backups = new();

        public BackupRestoreViewModel(IBackupService backupService, IExportService exportService, ILoggerService logger)
        {
            _backupService = backupService;
            _exportService = exportService;
            _logger = logger;
            Title = "Backup & Restore";
        }

        [RelayCommand]
        private async Task LoadBackupsAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var list = await _backupService.ListLocalBackupsAsync();
                Backups = new ObservableCollection<BackupInfoModel>(list);
            }, "Failed to load backups");
        }

        [RelayCommand]
        private async Task CreateBackupAsync()
        {
            await ExecuteBusyAsync(async () =>
            {
                var path = await _backupService.CreateLocalBackupAsync();
                await LoadBackupsAsync(); // Refresh list
                
                await Application.Current!.MainPage!.DisplayAlert("Success", "Backup created successfully.", "OK");
            }, "Failed to create backup");
        }

        [RelayCommand]
        private async Task RestoreAsync(string backupPath)
        {
            if (string.IsNullOrEmpty(backupPath)) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Restore Backup", 
                "Are you sure you want to restore this backup? This will overwrite your current database. The app will restart.", 
                "Restore", "Cancel");

            if (!confirm) return;

            await ExecuteBusyAsync(async () =>
            {
                var success = await _backupService.RestoreFromBackupAsync(backupPath);
                
                if (success)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Success", "Backup restored successfully.", "OK");
                    
                    // Navigate back to root Dashboard to reload data
                    await Shell.Current.GoToAsync("//dashboard");
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "Failed to restore backup.", "OK");
                }
            }, "Failed to restore backup");
        }

        [RelayCommand]
        private async Task DeleteAsync(string backupPath)
        {
            if (string.IsNullOrEmpty(backupPath)) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Backup", 
                "Are you sure you want to delete this backup permanently?", 
                "Delete", "Cancel");

            if (!confirm) return;

            await ExecuteBusyAsync(async () =>
            {
                await _backupService.DeleteBackupAsync(backupPath);
                await LoadBackupsAsync(); // Refresh list
            }, "Failed to delete backup");
        }
        
        [RelayCommand]
        private async Task ShareAsync(string backupPath)
        {
            if (string.IsNullOrEmpty(backupPath)) return;
            
            await ExecuteBusyAsync(async () =>
            {
                await _exportService.ShareFileAsync(backupPath, "Callyzer Backup");
            }, "Failed to share backup");
        }
    }
}
