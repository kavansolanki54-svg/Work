using Android.Content;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Provides type-safe access to Android SharedPreferences with encryption support.
    /// Sensitive values (tokens, secrets) are encrypted before storage.
    /// </summary>
    public class PreferenceService : IPreferenceService
    {
        private readonly ISharedPreferences _prefs;
        private readonly IEncryptionService _encryption;

        private static readonly System.Collections.Generic.HashSet<string> EncryptedKeys = new System.Collections.Generic.HashSet<string>
        {
            AppConstants.PrefKeyAccessToken,
            AppConstants.PrefKeyRefreshToken,
            AppConstants.PrefKeyEncryptionKey
        };

        /// <summary>
        /// Initializes a new instance of <see cref="PreferenceService"/>.
        /// </summary>
        /// <param name="context">The Android application context.</param>
        /// <param name="encryption">The encryption service for sensitive values.</param>
        public PreferenceService(Context context, IEncryptionService encryption)
        {
            _prefs = context.GetSharedPreferences(AppConstants.SecurePrefsName, FileCreationMode.Private)!;
            _encryption = encryption;
        }

        /// <inheritdoc />
        public void SetString(string key, string value)
        {
            var editor = _prefs.Edit()!;
            string storedValue = ShouldEncrypt(key) ? _encryption.Encrypt(value) : value;
            editor.PutString(key, storedValue);
            editor.Apply();
        }

        /// <inheritdoc />
        public string GetString(string key, string defaultValue = "")
        {
            string? raw = _prefs.GetString(key, null);
            if (raw == null) return defaultValue;
            return ShouldEncrypt(key) ? _encryption.Decrypt(raw) : raw;
        }

        /// <inheritdoc />
        public void SetInt(string key, int value)
        {
            var editor = _prefs.Edit()!;
            editor.PutInt(key, value);
            editor.Apply();
        }

        /// <inheritdoc />
        public int GetInt(string key, int defaultValue = 0)
        {
            return _prefs.GetInt(key, defaultValue);
        }

        /// <inheritdoc />
        public void SetLong(string key, long value)
        {
            var editor = _prefs.Edit()!;
            editor.PutLong(key, value);
            editor.Apply();
        }

        /// <inheritdoc />
        public long GetLong(string key, long defaultValue = 0L)
        {
            return _prefs.GetLong(key, defaultValue);
        }

        /// <inheritdoc />
        public void SetBool(string key, bool value)
        {
            var editor = _prefs.Edit()!;
            editor.PutBoolean(key, value);
            editor.Apply();
        }

        /// <inheritdoc />
        public bool GetBool(string key, bool defaultValue = false)
        {
            return _prefs.GetBoolean(key, defaultValue);
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            var editor = _prefs.Edit()!;
            editor.Remove(key);
            editor.Apply();
        }

        /// <inheritdoc />
        public bool Contains(string key)
        {
            return _prefs.Contains(key);
        }

        /// <inheritdoc />
        public void ClearAll()
        {
            var editor = _prefs.Edit()!;
            editor.Clear();
            editor.Apply();
        }

        /// <summary>
        /// Determines if a preference key contains sensitive data that should be encrypted.
        /// </summary>
        private bool ShouldEncrypt(string key)
        {
            return EncryptedKeys.Contains(key);
        }
    }
}
