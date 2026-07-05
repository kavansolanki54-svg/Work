namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for encrypted shared preferences operations.
    /// Provides type-safe get/set methods for all preference data types.
    /// </summary>
    public interface IPreferenceService
    {
        /// <summary>Stores a string value.</summary>
        void SetString(string key, string value);

        /// <summary>Retrieves a string value.</summary>
        string GetString(string key, string defaultValue = "");

        /// <summary>Stores an integer value.</summary>
        void SetInt(string key, int value);

        /// <summary>Retrieves an integer value.</summary>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>Stores a long value.</summary>
        void SetLong(string key, long value);

        /// <summary>Retrieves a long value.</summary>
        long GetLong(string key, long defaultValue = 0L);

        /// <summary>Stores a boolean value.</summary>
        void SetBool(string key, bool value);

        /// <summary>Retrieves a boolean value.</summary>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>Removes a specific preference key.</summary>
        void Remove(string key);

        /// <summary>Checks whether a key exists in preferences.</summary>
        bool Contains(string key);

        /// <summary>Clears all stored preferences (used during logout).</summary>
        void ClearAll();
    }
}
