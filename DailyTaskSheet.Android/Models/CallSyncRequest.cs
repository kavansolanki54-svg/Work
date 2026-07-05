using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Request payload for batch call log synchronization.
    /// Sent to POST /api/CallLogs/Sync.
    /// </summary>
    public class CallSyncRequest
    {
        /// <summary>Gets or sets the authenticated employee's ID.</summary>
        [JsonProperty("employeeId")]
        public int EmployeeId { get; set; }

        /// <summary>Gets or sets the company ID.</summary>
        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        /// <summary>Gets or sets the unique device identifier.</summary>
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Gets or sets the device display name.</summary>
        [JsonProperty("deviceName")]
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>Gets or sets the Android version string.</summary>
        [JsonProperty("androidVersion")]
        public string AndroidVersion { get; set; } = string.Empty;

        /// <summary>Gets or sets the application version.</summary>
        [JsonProperty("appVersion")]
        public string AppVersion { get; set; } = string.Empty;

        /// <summary>Gets or sets the list of call log records to sync.</summary>
        [JsonProperty("calls")]
        public List<CallSyncItem> Calls { get; set; } = new List<CallSyncItem>();
    }

    /// <summary>
    /// Individual call log item within a sync batch.
    /// Contains only the data needed by the API (not local-only fields).
    /// </summary>
    public class CallSyncItem
    {
        /// <summary>Gets or sets the local call log ID for correlation.</summary>
        [JsonProperty("callLogId")]
        public string CallLogId { get; set; } = string.Empty;

        /// <summary>Gets or sets the phone number.</summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>Gets or sets the contact name.</summary>
        [JsonProperty("contactName")]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>Gets or sets the call type name (Incoming, Outgoing, etc.).</summary>
        [JsonProperty("callType")]
        public string CallType { get; set; } = string.Empty;

        /// <summary>Gets or sets the call duration in seconds.</summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>Gets or sets the call date/time in ISO 8601 format.</summary>
        [JsonProperty("callDate")]
        public DateTime CallDate { get; set; }

        /// <summary>Gets or sets the call start time.</summary>
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        /// <summary>Gets or sets the call end time.</summary>
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        /// <summary>Gets or sets the country ISO code.</summary>
        [JsonProperty("countryIso")]
        public string CountryIso { get; set; } = string.Empty;

        /// <summary>Gets or sets the SIM slot index.</summary>
        [JsonProperty("simSlot")]
        public int SimSlot { get; set; }

        /// <summary>Gets or sets the raw Android call log ID.</summary>
        [JsonProperty("rawCallLogId")]
        public long RawCallLogId { get; set; }

        /// <summary>Gets or sets the sync hash for duplicate detection.</summary>
        [JsonProperty("syncHash")]
        public string SyncHash { get; set; } = string.Empty;
    }
}
