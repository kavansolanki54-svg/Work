using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Converters;
using DailyTaskSheet.App.Enums;
using DailyTaskSheet.App.Helpers;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Orchestrates the full call log synchronization cycle:
    /// 1. Read new call logs from Android
    /// 2. Store in local SQLite (deduplicated)
    /// 3. Upload pending records to the API in batches
    /// 4. Update sync status and history
    /// </summary>
    public class SyncService : ISyncService
    {
        private readonly ICallLogReaderService _callLogReader;
        private readonly ICallLogRepository _callLogRepo;
        private readonly ISyncRepository _syncRepo;
        private readonly IApiClient _apiClient;
        private readonly IAuthenticationService _authService;
        private readonly IDeviceService _deviceService;
        private readonly INotificationService _notificationService;
        private readonly INetworkService _networkService;
        private readonly IPreferenceService _preferenceService;
        private readonly ILoggerService _logger;
        private readonly INativeRecordingScannerService _recordingScanner;
        private static readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        /// <inheritdoc />
        public bool IsSyncing { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SyncService"/>.
        /// </summary>
        public SyncService(
            ICallLogReaderService callLogReader,
            ICallLogRepository callLogRepo,
            ISyncRepository syncRepo,
            IApiClient apiClient,
            IAuthenticationService authService,
            IDeviceService deviceService,
            INotificationService notificationService,
            INetworkService networkService,
            IPreferenceService preferenceService,
            ILoggerService logger,
            INativeRecordingScannerService recordingScanner)
        {
            _callLogReader = callLogReader;
            _callLogRepo = callLogRepo;
            _syncRepo = syncRepo;
            _apiClient = apiClient;
            _authService = authService;
            _deviceService = deviceService;
            _notificationService = notificationService;
            _networkService = networkService;
            _preferenceService = preferenceService;
            _logger = logger;
            _recordingScanner = recordingScanner;
        }

        /// <inheritdoc />
        public async Task<SyncHistoryModel> ExecuteSyncAsync(string triggerSource, CancellationToken cancellationToken = default)
        {
            if (!await _syncLock.WaitAsync(0, cancellationToken))
            {
                _logger.Warning("SyncService", "Sync already in progress, skipping.");
                return new SyncHistoryModel
                {
                    Status = (int)SyncStatusEnum.Failed,
                    Message = "Sync already in progress."
                };
            }

            IsSyncing = true;
            var stopwatch = Stopwatch.StartNew();
            var syncHistory = new SyncHistoryModel
            {
                SyncTime = DateTime.UtcNow,
                TriggerSource = triggerSource
            };

            try
            {
                _logger.Info("SyncService", $"Starting sync (triggered by: {triggerSource})");

                // Step 1: Check authentication
                if (!_authService.IsAuthenticated)
                {
                    _logger.Warning("SyncService", "User not authenticated. Skipping sync.");
                    syncHistory.Status = (int)SyncStatusEnum.Failed;
                    syncHistory.Message = "User not authenticated.";
                    return syncHistory;
                }

                // Refresh token if expired
                if (_authService.IsTokenExpired)
                {
                    bool refreshed = await _authService.RefreshTokenAsync(cancellationToken);
                    if (!refreshed)
                    {
                        syncHistory.Status = (int)SyncStatusEnum.Failed;
                        syncHistory.Message = "Token refresh failed.";
                        return syncHistory;
                    }
                }

                // Step 2: Read new call logs from Android
                long lastProcessedId = await _callLogRepo.GetLastProcessedRawIdAsync(cancellationToken);
                var newCallLogs = await _callLogReader.ReadNewCallLogsAsync(lastProcessedId, cancellationToken);
                _logger.Info("SyncService", $"Read {newCallLogs.Count} new call logs from device.");

                // Step 3: Store in SQLite (deduplicate)
                int stored = 0;
                foreach (var callLog in newCallLogs)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    bool exists = await _callLogRepo.ExistsByHashAsync(callLog.SyncHash, cancellationToken);
                    if (!exists)
                    {
                        await _callLogRepo.InsertAsync(callLog, cancellationToken);
                        stored++;
                    }
                }
                _logger.Info("SyncService", $"Stored {stored} new records ({newCallLogs.Count - stored} duplicates skipped).");

                // Step 4: Upload pending records to API
                if (!_networkService.IsConnected)
                {
                    _logger.Info("SyncService", "Offline — records queued for later upload.");
                    syncHistory.Status = (int)SyncStatusEnum.Pending;
                    syncHistory.TotalCalls = stored;
                    syncHistory.Message = "Offline. Records saved locally.";
                    return syncHistory;
                }

                // Check WiFi-only preference
                bool wifiOnly = _preferenceService.GetBool(AppConstants.PrefKeyWifiOnly, false);
                if (wifiOnly && !_networkService.IsWiFi)
                {
                    _logger.Info("SyncService", "WiFi-only mode enabled. Skipping upload on mobile data.");
                    syncHistory.Status = (int)SyncStatusEnum.Pending;
                    syncHistory.TotalCalls = stored;
                    syncHistory.Message = "WiFi-only mode. Records saved locally.";
                    return syncHistory;
                }

                // Upload in batches
                int totalUploaded = 0;
                int totalDuplicates = 0;
                int totalFailed = 0;

                var pendingRecords = await _callLogRepo.GetPendingForSyncAsync(AppConstants.MaxBatchSize, cancellationToken);
                syncHistory.TotalCalls = pendingRecords.Count;

                if (pendingRecords.Count == 0)
                {
                    _logger.Info("SyncService", "No pending records to upload.");
                    syncHistory.Status = (int)SyncStatusEnum.Synced;
                    syncHistory.Message = "No new records to synchronize.";
                    return syncHistory;
                }

                _notificationService.ShowSyncProgressNotification(0, pendingRecords.Count);

                // Mark as syncing
                var pendingIds = pendingRecords.Select(p => p.Id).ToList();
                await _callLogRepo.UpdateSyncStatusAsync(pendingIds, SyncStatusEnum.Syncing, cancellationToken);

                // Build the sync request
                var syncRequest = BuildSyncRequest(pendingRecords);
                var result = await _apiClient.PostAsync<CallSyncRequest, int>(
                    ApiEndpoints.SyncCallLogs, syncRequest, cancellationToken);

                if (result.Success)
                {
                    totalUploaded = result.Data;
                    totalDuplicates = 0; // API does not return this
                    totalFailed = 0;

                    // Mark all as synced
                    await _callLogRepo.UpdateSyncStatusAsync(pendingIds, SyncStatusEnum.Synced, cancellationToken);

                    _preferenceService.SetString(AppConstants.PrefKeyLastSyncTime,
                        DateTime.UtcNow.ToString("O"));

                    _notificationService.ShowSyncCompleteNotification(totalUploaded, totalDuplicates);
                    _logger.Info("SyncService",
                        $"Sync complete: Uploaded={totalUploaded}, Duplicates={totalDuplicates}, Failed={totalFailed}");

                    // 5. Check for Native Call Recordings and upload them
                    int empId = _authService.GetEmployeeId();
                    foreach (var record in pendingRecords)
                    {
                        try
                        {
                            var recordingPath = await _recordingScanner.FindRecordingPathAsync(record.PhoneNumber, record.StartTime);
                            if (!string.IsNullOrEmpty(recordingPath))
                            {
                                string endpoint = $"{ApiEndpoints.UploadNativeRecording}?employeeId={empId}&phoneNumber={Uri.EscapeDataString(record.PhoneNumber)}&startTime={Uri.EscapeDataString(record.StartTime.ToString("o"))}";
                                var uploadResult = await _apiClient.UploadFileAsync<string>(endpoint, recordingPath, "file", cancellationToken);
                                
                                if (uploadResult.Success)
                                {
                                    _logger.Info("SyncService", $"Uploaded recording for {record.PhoneNumber}");
                                }
                                else
                                {
                                    _logger.Warning("SyncService", $"Failed to upload recording: {uploadResult.Message}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning("SyncService", $"Exception while processing recording for {record.PhoneNumber}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    // Mark as failed for retry
                    await _callLogRepo.UpdateSyncStatusAsync(pendingIds, SyncStatusEnum.Failed, cancellationToken);
                    totalFailed = pendingRecords.Count;

                    string errorMsg = result.Message ?? "Unknown API error.";
                    _notificationService.ShowSyncFailedNotification(errorMsg);
                    _logger.Warning("SyncService", $"API sync failed: {errorMsg}");
                }

                syncHistory.Uploaded = totalUploaded;
                syncHistory.Duplicates = totalDuplicates;
                syncHistory.Failed = totalFailed;
                syncHistory.Status = totalFailed == 0 ? (int)SyncStatusEnum.Synced : (int)SyncStatusEnum.Failed;
                syncHistory.Message = result.Message ?? "Sync completed.";

                return syncHistory;
            }
            catch (global::System.OperationCanceledException)
            {
                _logger.Info("SyncService", "Sync cancelled.");
                syncHistory.Status = (int)SyncStatusEnum.Failed;
                syncHistory.Message = "Sync was cancelled.";
                return syncHistory;
            }
            catch (Exception ex)
            {
                _logger.Error("SyncService", "Sync failed with exception", ex);
                syncHistory.Status = (int)SyncStatusEnum.Failed;
                syncHistory.Message = $"Error: {ex.Message}";
                _notificationService.ShowSyncFailedNotification(ex.Message);
                return syncHistory;
            }
            finally
            {
                stopwatch.Stop();
                syncHistory.DurationMs = stopwatch.ElapsedMilliseconds;
                IsSyncing = false;
                _syncLock.Release();

                // Persist sync history
                try
                {
                    await _syncRepo.InsertAsync(syncHistory);
                }
                catch (Exception ex)
                {
                    _logger.Warning("SyncService", $"Failed to save sync history: {ex.Message}");
                }
            }
        }

        /// <inheritdoc />
        public async Task<int> RetryFailedSyncsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var failedRecords = await _callLogRepo.GetByStatusAsync(SyncStatusEnum.Failed, cancellationToken);
                if (failedRecords.Count == 0) return 0;

                _logger.Info("SyncService", $"Retrying {failedRecords.Count} failed records...");

                var syncHistory = await ExecuteSyncAsync("RetryFailed", cancellationToken);
                return syncHistory.Uploaded;
            }
            catch (Exception ex)
            {
                _logger.Error("SyncService", "Failed to retry syncs", ex);
                return 0;
            }
        }

        /// <summary>
        /// Builds a CallSyncRequest from pending call log records.
        /// </summary>
        private CallSyncRequest BuildSyncRequest(List<CallLogModel> records)
        {
            var deviceInfo = _deviceService.GetDeviceInfo();

            var request = new CallSyncRequest
            {
                EmployeeId = _authService.GetEmployeeId(),
                CompanyId = _authService.GetCompanyId(),
                DeviceId = deviceInfo.DeviceId,
                DeviceName = _deviceService.GetDeviceName(),
                AndroidVersion = deviceInfo.AndroidVersion,
                AppVersion = AppConstants.AppVersion,
                Calls = records.Select(r => new CallSyncItem
                {
                    CallLogId = r.Id.ToString(),
                    PhoneNumber = r.PhoneNumber,
                    ContactName = r.ContactName,
                    CallType = CallTypeConverter.ToApiString(r.CallTypeEnum),
                    Duration = r.Duration,
                    CallDate = r.CallDate,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    CountryIso = r.CountryIso,
                    SimId = r.SimSlot.ToString(),
                    RawCallLogId = r.RawCallLogId,
                    SyncHash = r.SyncHash
                }).ToList()
            };

            return request;
        }
    }
}
