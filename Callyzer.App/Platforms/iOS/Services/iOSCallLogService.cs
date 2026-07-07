using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.Platforms.iOS.Services
{
    /// <summary>
    /// iOS implementation of ICallLogPlatformService.
    /// iOS does NOT provide public API to read the device call log.
    /// This service uses CXCallObserver for real-time call detection only.
    /// </summary>
    public class iOSCallLogService : ICallLogPlatformService
    {
        private readonly ILoggerService _logger;

        /// <summary>
        /// iOS does NOT support reading historical call logs.
        /// Only real-time call observation via CXCallObserver is possible.
        /// </summary>
        public bool IsCallLogAccessSupported => false;

        public bool HasRequiredPermissions => true; // No special permissions needed for CXCallObserver

        public iOSCallLogService(ILoggerService logger)
        {
            _logger = logger;
            // TODO: Initialize CXCallObserver for real-time call detection
            // _callObserver = new CXCallObserver();
            // _callObserver.SetDelegate(new CallObserverDelegate(OnCallChanged), DispatchQueue.MainQueue);
        }

        public Task<bool> RequestPermissionsAsync()
        {
            // CXCallObserver doesn't require explicit permission
            _logger.Info("iOSCallLog", "iOS: No call log permission required (CXCallObserver)");
            return Task.FromResult(true);
        }

        public Task<List<CallLogModel>> ReadNewCallLogsAsync(
            long lastProcessedId, CancellationToken ct = default)
        {
            // iOS cannot read historical call logs
            // Return any calls observed in real-time since lastProcessedId
            _logger.Warning("iOSCallLog", "iOS: Historical call log access not available");
            return Task.FromResult(new List<CallLogModel>());
        }

        public Task<List<CallLogModel>> ReadCallLogsByDateAsync(
            long fromTimestamp, long toTimestamp, CancellationToken ct = default)
        {
            _logger.Warning("iOSCallLog", "iOS: Date-range call log query not supported");
            return Task.FromResult(new List<CallLogModel>());
        }

        public Task<int> GetTotalDeviceCallLogsAsync(CancellationToken ct = default)
        {
            return Task.FromResult(0);
        }
    }
}
