using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.SQLite;

namespace Callyzer.App.Services
{
    public class BackupService : IBackupService
    {
        private readonly DatabaseService _dbService;
        private readonly ILoggerService _logger;
        private readonly string _backupDirectory;

        public BackupService(DatabaseService dbService, ILoggerService logger)
        {
            _dbService = dbService;
            _logger = logger;
            _backupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Backups");
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        public async Task<string> CreateLocalBackupAsync(CancellationToken ct = default)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmm");
                var zipFileName = $"Callyzer_Backup_{timestamp}.zip";
                var zipFilePath = Path.Combine(_backupDirectory, zipFileName);

                var tempDbPath = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}.db3");
                
                // Copy the active DB to a temp location first to avoid locking issues while zipping
                File.Copy(_dbService.DatabasePath, tempDbPath, true);

                await Task.Run(() =>
                {
                    using var zipToOpen = new FileStream(zipFilePath, FileMode.Create);
                    using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);
                    archive.CreateEntryFromFile(tempDbPath, "CallyzerData.db3");
                }, ct);

                if (File.Exists(tempDbPath))
                {
                    File.Delete(tempDbPath);
                }

                _logger.Info("BackupService", $"Backup created successfully: {zipFilePath}");
                return zipFilePath;
            }
            catch (Exception ex)
            {
                _logger.Error("BackupService", "Failed to create backup", ex);
                throw new Exception("Failed to create backup", ex);
            }
        }

        public async Task<bool> RestoreFromBackupAsync(string backupPath, CancellationToken ct = default)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    _logger.Warning("BackupService", "Backup file not found.");
                    return false;
                }

                // 1. Close active DB connection
                await _dbService.CloseAsync();

                // 2. Extract backup to temporary file
                var tempExtractPath = Path.Combine(Path.GetTempPath(), $"extracted_{Guid.NewGuid()}.db3");
                
                await Task.Run(() =>
                {
                    using var archive = ZipFile.OpenRead(backupPath);
                    var dbEntry = archive.Entries.FirstOrDefault(e => e.Name == "CallyzerData.db3");
                    if (dbEntry != null)
                    {
                        dbEntry.ExtractToFile(tempExtractPath, true);
                    }
                }, ct);

                if (!File.Exists(tempExtractPath))
                {
                    _logger.Warning("BackupService", "Invalid backup archive: Database file missing.");
                    // Re-init DB
                    await _dbService.InitializeAsync();
                    return false;
                }

                // 3. Overwrite the active database file
                File.Copy(tempExtractPath, _dbService.DatabasePath, true);
                File.Delete(tempExtractPath);

                // 4. Re-initialize DB connection
                await _dbService.InitializeAsync();

                _logger.Info("BackupService", $"Backup restored successfully from: {backupPath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("BackupService", "Failed to restore backup", ex);
                
                // Try to ensure app isn't left in broken state
                await _dbService.InitializeAsync();
                return false;
            }
        }

        public Task<List<BackupInfoModel>> ListLocalBackupsAsync()
        {
            var backups = new List<BackupInfoModel>();
            try
            {
                var files = Directory.GetFiles(_backupDirectory, "*.zip");
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    backups.Add(new BackupInfoModel
                    {
                        FileName = fileInfo.Name,
                        FilePath = fileInfo.FullName,
                        CreatedAt = fileInfo.CreationTime,
                        FileSizeBytes = fileInfo.Length
                    });
                }
                
                return Task.FromResult(backups.OrderByDescending(b => b.CreatedAt).ToList());
            }
            catch (Exception ex)
            {
                _logger.Error("BackupService", "Failed to list backups", ex);
                return Task.FromResult(new List<BackupInfoModel>());
            }
        }

        public Task<bool> DeleteBackupAsync(string backupPath)
        {
            try
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                    _logger.Info("BackupService", $"Backup deleted: {backupPath}");
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.Error("BackupService", "Failed to delete backup", ex);
                return Task.FromResult(false);
            }
        }
    }
}
