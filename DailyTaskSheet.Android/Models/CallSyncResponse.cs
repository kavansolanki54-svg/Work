using Newtonsoft.Json;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Response payload from the call log synchronization endpoint.
    /// Returned from POST /api/CallLogs/Sync.
    /// </summary>
    public class CallSyncResponse
    {
        /// <summary>Gets or sets whether the sync operation was successful.</summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>Gets or sets the number of records successfully uploaded.</summary>
        [JsonProperty("uploaded")]
        public int Uploaded { get; set; }

        /// <summary>Gets or sets the number of duplicate records detected by the server.</summary>
        [JsonProperty("duplicates")]
        public int Duplicates { get; set; }

        /// <summary>Gets or sets the number of records that failed to upload.</summary>
        [JsonProperty("failed")]
        public int Failed { get; set; }

        /// <summary>Gets or sets the human-readable result message.</summary>
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
