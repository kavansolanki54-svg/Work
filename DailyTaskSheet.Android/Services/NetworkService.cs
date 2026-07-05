using System;
using Android.Content;
using Android.Net;
using Android.OS;
using DailyTaskSheet.App.Enums;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Monitors network connectivity using Android's ConnectivityManager.
    /// Raises events on connectivity changes for sync decisions.
    /// </summary>
    public class NetworkService : INetworkService
    {
        private readonly Context _context;
        private readonly ConnectivityManager _connectivityManager;
        private readonly ILoggerService _logger;
        private ConnectivityManager.NetworkCallback? _networkCallback;

        /// <inheritdoc />
        public NetworkStatusEnum CurrentStatus { get; private set; } = NetworkStatusEnum.Offline;

        /// <inheritdoc />
        public bool IsConnected => CurrentStatus != NetworkStatusEnum.Offline;

        /// <inheritdoc />
        public bool IsWiFi => CurrentStatus == NetworkStatusEnum.WiFi;

        /// <inheritdoc />
        public bool IsMobileData => CurrentStatus == NetworkStatusEnum.MobileData;

        /// <inheritdoc />
        public event EventHandler<NetworkStatusEnum>? NetworkStatusChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="NetworkService"/>.
        /// </summary>
        public NetworkService(Context context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
            _connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService)!;
            UpdateCurrentStatus();
        }

        /// <inheritdoc />
        public void StartMonitoring()
        {
            try
            {
                if (_networkCallback != null) return;

                _networkCallback = new SyncNetworkCallback(this, _logger);
                var request = new NetworkRequest.Builder()!
                    .AddCapability(NetCapability.Internet)!
                    .Build()!;

                _connectivityManager.RegisterNetworkCallback(request, _networkCallback);
                _logger.Info("NetworkService", "Network monitoring started.");
            }
            catch (Exception ex)
            {
                _logger.Error("NetworkService", "Failed to start network monitoring", ex);
            }
        }

        /// <inheritdoc />
        public void StopMonitoring()
        {
            try
            {
                if (_networkCallback != null)
                {
                    _connectivityManager.UnregisterNetworkCallback(_networkCallback);
                    _networkCallback = null;
                    _logger.Info("NetworkService", "Network monitoring stopped.");
                }
            }
            catch (Exception ex)
            {
                _logger.Warning("NetworkService", $"Error stopping network monitoring: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the current network status by querying ConnectivityManager.
        /// </summary>
        internal void UpdateCurrentStatus()
        {
            try
            {
                var activeNetwork = _connectivityManager.ActiveNetwork;
                if (activeNetwork == null)
                {
                    SetStatus(NetworkStatusEnum.Offline);
                    return;
                }

                var capabilities = _connectivityManager.GetNetworkCapabilities(activeNetwork);
                if (capabilities == null)
                {
                    SetStatus(NetworkStatusEnum.Offline);
                    return;
                }

                if (capabilities.HasTransport(TransportType.Wifi))
                {
                    SetStatus(NetworkStatusEnum.WiFi);
                }
                else if (capabilities.HasTransport(TransportType.Cellular))
                {
                    SetStatus(NetworkStatusEnum.MobileData);
                }
                else if (capabilities.HasTransport(TransportType.Ethernet))
                {
                    SetStatus(NetworkStatusEnum.Ethernet);
                }
                else
                {
                    SetStatus(NetworkStatusEnum.Offline);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning("NetworkService", $"Error checking network status: {ex.Message}");
                SetStatus(NetworkStatusEnum.Offline);
            }
        }

        /// <summary>
        /// Sets the current status and raises the change event if it changed.
        /// </summary>
        internal void SetStatus(NetworkStatusEnum newStatus)
        {
            if (CurrentStatus != newStatus)
            {
                var oldStatus = CurrentStatus;
                CurrentStatus = newStatus;
                _logger.Info("NetworkService", $"Network status changed: {oldStatus} → {newStatus}");
                NetworkStatusChanged?.Invoke(this, newStatus);
            }
        }

        /// <summary>
        /// Network callback for connectivity changes.
        /// </summary>
        private class SyncNetworkCallback : ConnectivityManager.NetworkCallback
        {
            private readonly NetworkService _service;
            private readonly ILoggerService _logger;

            public SyncNetworkCallback(NetworkService service, ILoggerService logger)
            {
                _service = service;
                _logger = logger;
            }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                _service.UpdateCurrentStatus();
            }

            public override void OnLost(Network network)
            {
                base.OnLost(network);
                _service.SetStatus(NetworkStatusEnum.Offline);
            }

            public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities)
            {
                base.OnCapabilitiesChanged(network, networkCapabilities);
                _service.UpdateCurrentStatus();
            }
        }
    }
}
