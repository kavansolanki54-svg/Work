using System.Threading;
using System.Threading.Tasks;
using Callyzer.App.Enums;

namespace Callyzer.App.Interfaces
{
    /// <summary>
    /// Interface for export operations (CSV/PDF).
    /// </summary>
    public interface IExportService
    {
        Task<string> ExportToCsvAsync(System.DateTime from, System.DateTime to, CancellationToken ct = default);
        Task<string> ExportToPdfAsync(Models.AnalyticsSummaryModel summary, System.Collections.Generic.List<Models.ContactAnalyticsModel> topContacts, CancellationToken ct = default);
        Task ShareFileAsync(string filePath, string title);
    }

    /// <summary>
    /// Interface for backup and restore operations.
    /// </summary>
    public interface IBackupService
    {
        Task<string> CreateLocalBackupAsync(CancellationToken ct = default);
        Task<bool> RestoreFromBackupAsync(string backupPath, CancellationToken ct = default);
        Task<System.Collections.Generic.List<Models.BackupInfoModel>> ListLocalBackupsAsync();
        Task<bool> DeleteBackupAsync(string backupPath);
    }

    /// <summary>
    /// Interface for device information retrieval (platform-specific).
    /// </summary>
    public interface IDeviceService
    {
        string GetDeviceId();
        string GetDeviceName();
        int GetBatteryPercentage();
        string GetTimeZone();
        Models.DeviceInformation GetDeviceInfo();
    }

    /// <summary>
    /// Interface for encryption/decryption.
    /// </summary>
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    /// <summary>
    /// Interface for preference/settings storage (cross-platform).
    /// </summary>
    public interface IPreferenceService
    {
        string GetString(string key, string defaultValue = "");
        void SetString(string key, string value);
        int GetInt(string key, int defaultValue = 0);
        void SetInt(string key, int value);
        bool GetBool(string key, bool defaultValue = false);
        void SetBool(string key, bool value);
        void Remove(string key);
        void Clear();
        
        Task<string> GetSecureStringAsync(string key, string defaultValue = "");
        Task SetSecureStringAsync(string key, string value);
    }

    /// <summary>
    /// Interface for network connectivity status (cross-platform).
    /// </summary>
    public interface INetworkService
    {
        bool IsConnected { get; }
        bool IsWiFi { get; }
        NetworkStatusEnum CurrentStatus { get; }
    }
}
