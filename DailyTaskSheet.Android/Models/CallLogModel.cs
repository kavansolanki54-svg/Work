using System;
using SQLite;
using DailyTaskSheet.App.Enums;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Represents a single call log record captured from the Android call log provider.
    /// Stored in the local SQLite database with all available metadata.
    /// </summary>
    [Table("CallLogs")]
    public class CallLogModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>The raw Android call log ID from CallLog.Calls._ID.</summary>
        [Indexed]
        public long RawCallLogId { get; set; }

        /// <summary>The phone number associated with the call.</summary>
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>The contact name from the contacts database (cached).</summary>
        [MaxLength(200)]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>The type of call (Incoming, Outgoing, Missed, etc.).</summary>
        public int CallType { get; set; }

        /// <summary>Gets the call type as a strongly typed enum.</summary>
        [Ignore]
        public CallTypeEnum CallTypeEnum
        {
            get => (CallTypeEnum)CallType;
            set => CallType = (int)value;
        }

        /// <summary>Call duration in seconds. Zero for missed/rejected calls.</summary>
        public int Duration { get; set; }

        /// <summary>Ring duration in seconds before the call was answered or ended.</summary>
        public int RingDuration { get; set; }

        /// <summary>The date and time the call was placed or received.</summary>
        [Indexed]
        public DateTime CallDate { get; set; }

        /// <summary>The start time of the call (same as CallDate for most devices).</summary>
        public DateTime StartTime { get; set; }

        /// <summary>The end time of the call (CallDate + Duration).</summary>
        public DateTime EndTime { get; set; }

        /// <summary>Cached name label from the contacts database.</summary>
        [MaxLength(200)]
        public string CachedName { get; set; } = string.Empty;

        /// <summary>Cached number label (e.g., "Mobile", "Work").</summary>
        [MaxLength(50)]
        public string CachedNumberLabel { get; set; } = string.Empty;

        /// <summary>Cached number type from the contacts database.</summary>
        public int CachedNumberType { get; set; }

        /// <summary>ISO country code for the phone number.</summary>
        [MaxLength(10)]
        public string CountryIso { get; set; } = string.Empty;

        /// <summary>Geocoded location for the phone number.</summary>
        [MaxLength(200)]
        public string GeocodedLocation { get; set; } = string.Empty;

        /// <summary>Presentation type (ALLOWED, RESTRICTED, UNKNOWN, PAYPHONE).</summary>
        public int Presentation { get; set; }

        /// <summary>Phone account component name (identifies the SIM/account).</summary>
        [MaxLength(200)]
        public string PhoneAccountComponent { get; set; } = string.Empty;

        /// <summary>Phone account ID.</summary>
        [MaxLength(200)]
        public string PhoneAccountId { get; set; } = string.Empty;

        /// <summary>Subscription ID identifying the SIM card.</summary>
        public int SubscriptionId { get; set; }

        /// <summary>SIM slot index (0-based). -1 if unknown.</summary>
        public int SimSlot { get; set; } = -1;

        /// <summary>Whether the call log entry has been read.</summary>
        public bool IsRead { get; set; }

        /// <summary>Whether this is a new (unread) call log entry.</summary>
        public bool IsNew { get; set; }

        /// <summary>Unique device identifier for multi-device tracking.</summary>
        [MaxLength(100)]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Device manufacturer (e.g., "Samsung").</summary>
        [MaxLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>Device model (e.g., "SM-S911B").</summary>
        [MaxLength(100)]
        public string DeviceModel { get; set; } = string.Empty;

        /// <summary>Android version string (e.g., "14").</summary>
        [MaxLength(20)]
        public string AndroidVersion { get; set; } = string.Empty;

        /// <summary>Battery percentage at the time of capture (0-100).</summary>
        public int BatteryPercentage { get; set; }

        /// <summary>Device timezone at the time of capture.</summary>
        [MaxLength(50)]
        public string TimeZone { get; set; } = string.Empty;

        /// <summary>Synchronization status of this record.</summary>
        [Indexed]
        public int SyncStatus { get; set; } = (int)SyncStatusEnum.Pending;

        /// <summary>Gets the sync status as a strongly typed enum.</summary>
        [Ignore]
        public SyncStatusEnum SyncStatusEnum
        {
            get => (SyncStatusEnum)SyncStatus;
            set => SyncStatus = (int)value;
        }

        /// <summary>SHA256 hash for duplicate detection.</summary>
        [MaxLength(64), Indexed]
        public string SyncHash { get; set; } = string.Empty;

        /// <summary>Timestamp when this record was first captured locally.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Timestamp when this record was last modified.</summary>
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
