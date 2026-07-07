using Callyzer.App.Enums;
using Callyzer.App.Interfaces;

namespace Callyzer.App.Services
{
    /// <summary>
    /// Cross-platform network connectivity service using MAUI Connectivity API.
    /// </summary>
    public class NetworkService : INetworkService
    {
        public bool IsConnected =>
            Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        public bool IsWiFi =>
            Connectivity.Current.ConnectionProfiles.Contains(ConnectionProfile.WiFi);

        public NetworkStatusEnum CurrentStatus
        {
            get
            {
                if (!IsConnected) return NetworkStatusEnum.Offline;
                var profiles = Connectivity.Current.ConnectionProfiles;
                if (profiles.Contains(ConnectionProfile.WiFi)) return NetworkStatusEnum.WiFi;
                if (profiles.Contains(ConnectionProfile.Cellular)) return NetworkStatusEnum.MobileData;
                if (profiles.Contains(ConnectionProfile.Ethernet)) return NetworkStatusEnum.Ethernet;
                return NetworkStatusEnum.Offline;
            }
        }
    }
}
