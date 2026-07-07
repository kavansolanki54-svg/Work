using Android.Content;
using AndroidX.Work;
using Callyzer.App.Interfaces;

namespace Callyzer.App.Platforms.Android.Services
{
    public class SyncWorker : Worker
    {
        private readonly ISyncService _syncService;
        private readonly ILoggerService _logger;

        public SyncWorker(Context context, WorkerParameters workerParams) 
            : base(context, workerParams)
        {
            // In a real MAUI app, we'd resolve these via IServiceProvider
            // or IPlatformApplication.Current.Services.
            // For simplicity here, assuming they are available globally or instantiated
            _syncService = Microsoft.Maui.IPlatformApplication.Current!.Services.GetService(typeof(ISyncService)) as ISyncService;
            _logger = Microsoft.Maui.IPlatformApplication.Current!.Services.GetService(typeof(ILoggerService)) as ILoggerService;
        }

        public override Result DoWork()
        {
            _logger.Info("SyncWorker", "Background sync triggered by WorkManager");
            
            try
            {
                // Run sync synchronously for the Worker thread
                var success = _syncService.SyncPendingLogsAsync(isBackground: true).GetAwaiter().GetResult();
                return success ? Result.InvokeSuccess() : Result.InvokeRetry();
            }
            catch (System.Exception ex)
            {
                _logger.Error("SyncWorker", "Background sync failed", ex);
                return Result.InvokeFailure();
            }
        }
    }
}
