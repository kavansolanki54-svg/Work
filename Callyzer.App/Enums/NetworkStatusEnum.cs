namespace Callyzer.App.Enums
{
    /// <summary>
    /// Represents the current network connectivity status of the device.
    /// Used to determine whether synchronization should proceed.
    /// </summary>
    public enum NetworkStatusEnum
    {
        /// <summary>Device has no network connection.</summary>
        Offline = 0,

        /// <summary>Device is connected via WiFi.</summary>
        WiFi = 1,

        /// <summary>Device is connected via mobile/cellular data.</summary>
        MobileData = 2,

        /// <summary>Device is connected via Ethernet.</summary>
        Ethernet = 3
    }
}
