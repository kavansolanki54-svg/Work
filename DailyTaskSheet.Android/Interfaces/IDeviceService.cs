using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for retrieving device hardware and software information.
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>Retrieves comprehensive device information.</summary>
        /// <returns>A populated DeviceInformation model.</returns>
        DeviceInformation GetDeviceInfo();

        /// <summary>Gets the unique device identifier (ANDROID_ID).</summary>
        string GetDeviceId();

        /// <summary>Gets a human-readable device name (e.g., "Samsung Galaxy S23").</summary>
        string GetDeviceName();

        /// <summary>Gets the current battery percentage (0-100).</summary>
        int GetBatteryPercentage();

        /// <summary>Gets the current timezone identifier.</summary>
        string GetTimeZone();
    }
}
