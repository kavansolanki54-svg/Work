using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Constants;
using Callyzer.App.Enums;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.Models.Api;
using Callyzer.App.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Callyzer.App.Services
{
    public class SyncService : ISyncService
    {
        private readonly CallLogRepository _callLogRepo;
        private readonly INetworkService _networkService;
        private readonly IPreferenceService _preferenceService;
        private readonly ILoggerService _logger;
        private readonly HttpClient _httpClient;

        private bool _isSyncing;
        private const int BatchSize = 100;
        private const string Tag = "SyncService";
        private readonly AsyncRetryPolicy _retryPolicy;

        public event EventHandler<SyncProgressEventArgs>? SyncProgressChanged;

        public bool IsSyncing
        {
            get => _isSyncing;
            private set
            {
                _isSyncing = value;
                OnSyncProgressChanged(new SyncProgressEventArgs { IsSyncing = value });
            }
        }

        public SyncService(
            CallLogRepository callLogRepo,
            INetworkService networkService,
            IPreferenceService preferenceService,
            ILoggerService logger)
        {
            _callLogRepo = callLogRepo;
            _networkService = networkService;
            _preferenceService = preferenceService;
            _logger = logger;
            
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.Warning(Tag, $"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                    });
        }

        public async Task<int> GetPendingSyncCountAsync(CancellationToken ct = default)
        {
            var logs = await _callLogRepo.GetPendingSyncAsync(0); // 0 limit means all for count
            return logs.Count;
        }

        public async Task<bool> SyncPendingLogsAsync(bool isBackground = false, CancellationToken ct = default)
        {
            if (IsSyncing)
            {
                _logger.Warning(Tag, "Sync already in progress. Ignoring trigger.");
                return false;
            }

            if (!_networkService.IsConnected)
            {
                _logger.Warning(Tag, "Cannot sync: No internet connection");
                return false;
            }

            // Check wifi-only setting
            if (isBackground && _preferenceService.GetBool("WifiOnlySync", false) && 
                !_networkService.IsWiFi)
            {
                _logger.Info(Tag, "Skipping background sync: Wi-Fi required");
                return false;
            }

            try
            {
                IsSyncing = true;
                
                // 1. Get all pending logs
                var pendingLogs = await _callLogRepo.GetPendingSyncAsync(500); // Max 500 per session to avoid timeouts
                
                if (pendingLogs.Count == 0)
                {
                    _logger.Info(Tag, "No pending logs to sync");
                    return true;
                }

                _logger.Info(Tag, $"Starting sync for {pendingLogs.Count} logs in batches of {BatchSize}");
                
                int totalSynced = 0;
                
                // 2. Process in batches
                for (int i = 0; i < pendingLogs.Count; i += BatchSize)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    var batch = pendingLogs.Skip(i).Take(BatchSize).ToList();
                    
                    OnSyncProgressChanged(new SyncProgressEventArgs
                    {
                        IsSyncing = true,
                        TotalToSync = pendingLogs.Count,
                        SyncedSoFar = totalSynced,
                        Message = $"Syncing batch {i / BatchSize + 1}..."
                    });

                    // 3. Send to API
                    bool batchSuccess = await SendBatchToApiAsync(batch, ct);
                    
                    if (batchSuccess)
                    {
                        // 4. Update local DB status
                        foreach (var log in batch)
                        {
                            log.SyncStatus = (int)SyncStatusEnum.Synced;
                            log.ModifiedAt = DateTime.UtcNow;
                        }
                        foreach (var log in batch)
                        {
                            await _callLogRepo.UpdateAsync(log);
                        }
                        totalSynced += batch.Count;
                    }
                    else
                    {
                        _logger.Error(Tag, $"Failed to sync batch {i / BatchSize + 1}. Aborting sync session.");
                        return false;
                    }
                }

                _logger.Info(Tag, $"Successfully synced {totalSynced} logs");
                
                WeakReferenceMessenger.Default.Send(new Callyzer.App.Models.Messages.SyncCompletedMessage 
                { 
                    Success = true, 
                    SyncedCount = totalSynced 
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(Tag, "Error during sync operation", ex);
                return false;
            }
            finally
            {
                IsSyncing = false;
            }
        }

        private async Task<bool> SendBatchToApiAsync(List<CallLogModel> batch, CancellationToken ct)
        {
            try
            {
                var token = await _preferenceService.GetSecureStringAsync(Constants.AppConstants.PrefKeyAccessToken, string.Empty);
                if (string.IsNullOrEmpty(token))
                {
                    _logger.Error(Tag, "Cannot sync: No auth token found");
                    return false;
                }

                var baseUrl = _preferenceService.GetString(Constants.AppConstants.PrefKeyApiBaseUrl, Constants.AppConstants.DefaultApiBaseUrl);
                var requestUrl = $"{baseUrl.TrimEnd('/')}{ApiEndpoints.SyncCallLogs}";

                var payload = new SyncPayloadModel
                {
                    CallLogs = batch.Select(c => new CallLogSyncModel
                    {
                        PhoneNumber = c.PhoneNumber,
                        ContactName = c.ContactName,
                        CallType = GetCallTypeString(c.CallType),
                        StartTime = c.CallDate,
                        EndTime = c.CallDate.AddSeconds(c.Duration),
                        DurationInSeconds = c.Duration,
                        SimId = c.SimSlot.ToString()
                    }).ToList()
                };

                var json = JsonConvert.SerializeObject(payload);

                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                    request.Headers.Add("Authorization", $"Bearer {token}");
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request, ct);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync(ct);
                        _logger.Error(Tag, $"API Error ({response.StatusCode}): {errorContent}");
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(Tag, "Exception sending batch to API", ex);
                return false;
            }
        }

        protected virtual void OnSyncProgressChanged(SyncProgressEventArgs e)
        {
            SyncProgressChanged?.Invoke(this, e);
        }

        private string GetCallTypeString(int type) => type switch
        {
            1 => "Incoming",
            2 => "Outgoing",
            3 => "Missed",
            5 => "Rejected",
            _ => "Unknown"
        };
    }
}
