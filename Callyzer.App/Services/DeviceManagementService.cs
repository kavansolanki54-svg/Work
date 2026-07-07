using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Callyzer.App.Constants;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Microsoft.Maui.Devices;
using Newtonsoft.Json;

namespace Callyzer.App.Services
{
    public class DeviceManagementService : IDeviceManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly IPreferenceService _preferenceService;
        private readonly ILoggerService _logger;

        public DeviceManagementService(IPreferenceService preferenceService, ILoggerService logger)
        {
            _preferenceService = preferenceService;
            _logger = logger;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        }

        public async Task RegisterDeviceAsync()
        {
            try
            {
                var token = await _preferenceService.GetSecureStringAsync(AppConstants.PrefKeyAccessToken, string.Empty);
                if (string.IsNullOrEmpty(token)) return; // Only register if logged in

                var baseUrl = _preferenceService.GetString(AppConstants.PrefKeyApiBaseUrl, AppConstants.DefaultApiBaseUrl);
                var requestUrl = $"{baseUrl.TrimEnd('/')}{ApiEndpoints.DeviceRegister}";

                var deviceId = _preferenceService.GetString("DeviceId", string.Empty);
                if (string.IsNullOrEmpty(deviceId))
                {
                    deviceId = Guid.NewGuid().ToString();
                    _preferenceService.SetString("DeviceId", deviceId);
                }

                var deviceInfo = new DeviceInformation
                {
                    DeviceId = deviceId,
                    Manufacturer = DeviceInfo.Manufacturer,
                    Model = DeviceInfo.Model,
                    OsVersion = DeviceInfo.VersionString,
                    AppVersion = AppInfo.VersionString,
                    Platform = DeviceInfo.Platform.ToString(),
                    BatteryPercentage = Battery.ChargeLevel > 0 ? (int)(Battery.ChargeLevel * 100) : 100,
                    TimeZone = TimeZoneInfo.Local.Id
                };

                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = new StringContent(JsonConvert.SerializeObject(deviceInfo), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.Info("DeviceManagement", "Device registered/updated successfully.");
                }
                else
                {
                    _logger.Warning("DeviceManagement", $"Failed to register device: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DeviceManagement", "Exception during device registration", ex);
            }
        }
    }
}
