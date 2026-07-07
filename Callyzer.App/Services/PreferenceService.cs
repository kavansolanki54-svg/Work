using Callyzer.App.Interfaces;

namespace Callyzer.App.Services
{
    /// <summary>
    /// Cross-platform preference service using MAUI Preferences API.
    /// Replaces Android SharedPreferences with platform-agnostic storage.
    /// </summary>
    public class PreferenceService : IPreferenceService
    {
        public string GetString(string key, string defaultValue = "")
            => Preferences.Get(key, defaultValue);

        public void SetString(string key, string value)
            => Preferences.Set(key, value);

        public int GetInt(string key, int defaultValue = 0)
            => Preferences.Get(key, defaultValue);

        public void SetInt(string key, int value)
            => Preferences.Set(key, value);

        public bool GetBool(string key, bool defaultValue = false)
            => Preferences.Get(key, defaultValue);

        public void SetBool(string key, bool value)
            => Preferences.Set(key, value);

        public void Remove(string key)
            => Preferences.Remove(key);

        public void Clear()
            => Preferences.Clear();
            
        public async Task<string> GetSecureStringAsync(string key, string defaultValue = "")
        {
            try
            {
                var value = await SecureStorage.Default.GetAsync(key);
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch
            {
                return defaultValue; // Fallback in case secure storage is unavailable (e.g. some Android emulators)
            }
        }

        public async Task SetSecureStringAsync(string key, string value)
        {
            try
            {
                await SecureStorage.Default.SetAsync(key, value);
            }
            catch
            {
                // Fallback in case secure storage is unavailable
                Preferences.Set(key, value); 
            }
        }
    }
}
