using System;
using Android.Content;
using Android.OS;
using Android.Provider;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Retrieves device hardware, software, and runtime information.
    /// Used for API payloads and local diagnostics.
    /// </summary>
    public class DeviceService : IDeviceService
    {
        private readonly Context _context;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="DeviceService"/>.
        /// </summary>
        public DeviceService(Context context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public DeviceInformation GetDeviceInfo()
        {
            return new DeviceInformation
            {
                DeviceId = GetDeviceId(),
                Manufacturer = Build.Manufacturer ?? "Unknown",
                Model = Build.Model ?? "Unknown",
                Brand = Build.Brand ?? "Unknown",
                AndroidVersion = Build.VERSION.Release ?? "Unknown",
                ApiLevel = (int)Build.VERSION.SdkInt,
                BuildDisplay = Build.Display ?? "Unknown",
                Product = Build.Product ?? "Unknown",
                Hardware = Build.Hardware ?? "Unknown",
                ScreenDensityDpi = GetScreenDensityDpi(),
                ScreenWidthPixels = GetScreenWidthPixels(),
                ScreenHeightPixels = GetScreenHeightPixels(),
                TotalRamMb = GetTotalRamMb(),
                LastUpdated = DateTime.UtcNow
            };
        }

        /// <inheritdoc />
        public string GetDeviceId()
        {
            try
            {
                return Settings.Secure.GetString(_context.ContentResolver, Settings.Secure.AndroidId) ?? "UNKNOWN";
            }
            catch (Exception ex)
            {
                _logger.Warning("DeviceService", $"Could not get ANDROID_ID: {ex.Message}");
                return "UNKNOWN";
            }
        }

        /// <inheritdoc />
        public string GetDeviceName()
        {
            string manufacturer = Build.Manufacturer ?? "";
            string model = Build.Model ?? "";

            if (model.StartsWith(manufacturer, StringComparison.OrdinalIgnoreCase))
            {
                return model;
            }
            return $"{manufacturer} {model}".Trim();
        }

        /// <inheritdoc />
        public int GetBatteryPercentage()
        {
            try
            {
                var batteryManager = (BatteryManager?)_context.GetSystemService(Context.BatteryService);
                if (batteryManager != null)
                {
                    return batteryManager.GetIntProperty((int)BatteryProperty.Capacity);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning("DeviceService", $"Could not get battery level: {ex.Message}");
            }
            return -1;
        }

        /// <inheritdoc />
        public string GetTimeZone()
        {
            return Java.Util.TimeZone.Default?.ID ?? TimeZoneInfo.Local.Id;
        }

        private int GetScreenDensityDpi()
        {
            try
            {
                return (int)(_context.Resources?.DisplayMetrics?.DensityDpi ?? 0);
            }
            catch { return 0; }
        }

        private int GetScreenWidthPixels()
        {
            try
            {
                return _context.Resources?.DisplayMetrics?.WidthPixels ?? 0;
            }
            catch { return 0; }
        }

        private int GetScreenHeightPixels()
        {
            try
            {
                return _context.Resources?.DisplayMetrics?.HeightPixels ?? 0;
            }
            catch { return 0; }
        }

        private long GetTotalRamMb()
        {
            try
            {
                var activityManager = (Android.App.ActivityManager?)_context.GetSystemService(Context.ActivityService);
                if (activityManager != null)
                {
                    var memInfo = new Android.App.ActivityManager.MemoryInfo();
                    activityManager.GetMemoryInfo(memInfo);
                    return memInfo.TotalMem / (1024 * 1024);
                }
            }
            catch { }
            return 0;
        }
    }
}
