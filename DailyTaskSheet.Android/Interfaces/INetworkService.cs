using System;
using DailyTaskSheet.App.Enums;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for monitoring network connectivity.
    /// Provides real-time network status and change notifications.
    /// </summary>
    public interface INetworkService
    {
        /// <summary>Gets the current network connectivity status.</summary>
        NetworkStatusEnum CurrentStatus { get; }

        /// <summary>Gets whether the device currently has any network connection.</summary>
        bool IsConnected { get; }

        /// <summary>Gets whether the device is connected via WiFi.</summary>
        bool IsWiFi { get; }

        /// <summary>Gets whether the device is connected via mobile data.</summary>
        bool IsMobileData { get; }

        /// <summary>Starts monitoring network changes.</summary>
        void StartMonitoring();

        /// <summary>Stops monitoring network changes.</summary>
        void StopMonitoring();

        /// <summary>Event raised when network connectivity changes.</summary>
        event EventHandler<NetworkStatusEnum>? NetworkStatusChanged;
    }
}
