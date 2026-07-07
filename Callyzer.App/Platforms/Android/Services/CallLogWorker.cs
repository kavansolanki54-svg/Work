using Android.Content;
using AndroidX.Work;
using Callyzer.App.Interfaces;

namespace Callyzer.App.Platforms.Android.Services
{
    public class CallLogWorker : Worker
    {
        private readonly ICallLogPlatformService _callLogService;
        private readonly ILoggerService _logger;

        public CallLogWorker(Context context, WorkerParameters workerParams) 
            : base(context, workerParams)
        {
            _callLogService = Microsoft.Maui.IPlatformApplication.Current!.Services.GetService(typeof(ICallLogPlatformService)) as ICallLogPlatformService;
            _logger = Microsoft.Maui.IPlatformApplication.Current!.Services.GetService(typeof(ILoggerService)) as ILoggerService;
        }

        public override Result DoWork()
        {
            _logger.Info("CallLogWorker", "Background call log ingestion triggered by WorkManager");
            
            try
            {
                if (!_callLogService.HasRequiredPermissions)
                {
                    _logger.Warning("CallLogWorker", "Cannot ingest call logs without permissions.");
                    return Result.InvokeFailure();
                }

                // Ingest latest logs synchronously
                var callLogRepo = Microsoft.Maui.IPlatformApplication.Current!.Services.GetService(typeof(Callyzer.App.Repositories.CallLogRepository)) as Callyzer.App.Repositories.CallLogRepository;
                
                if (callLogRepo != null && _callLogService != null)
                {
                    var lastId = callLogRepo.GetLastRawCallLogIdAsync().GetAwaiter().GetResult();
                    var newLogs = _callLogService.ReadNewCallLogsAsync(lastId).GetAwaiter().GetResult();
                    
                    if (newLogs.Count > 0)
                    {
                        callLogRepo.InsertAllAsync(newLogs).GetAwaiter().GetResult();
                        _logger.Info("CallLogWorker", $"Successfully ingested {newLogs.Count} new call logs in background.");
                    }
                    else
                    {
                        _logger.Info("CallLogWorker", "No new call logs to ingest.");
                    }
                }
                
                return Result.InvokeSuccess();
            }
            catch (System.Exception ex)
            {
                _logger.Error("CallLogWorker", "Background call log ingestion failed", ex);
                return Result.InvokeFailure();
            }
        }
    }
}
