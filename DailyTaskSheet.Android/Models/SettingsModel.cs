using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Key-value settings stored in the local SQLite database.
    /// Supports both local preferences and server-pushed settings.
    /// </summary>
    [Table("Settings")]
    public class SettingsModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Unique setting key.</summary>
        [MaxLength(100), Indexed(Unique = true)]
        public string Key { get; set; } = string.Empty;

        /// <summary>Setting value (stored as string, parsed as needed).</summary>
        [MaxLength(1000)]
        public string Value { get; set; } = string.Empty;

        /// <summary>Setting description for display purposes.</summary>
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Whether this setting was pushed from the server.</summary>
        public bool IsServerSetting { get; set; }

        /// <summary>Timestamp when the setting was last updated.</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
