using System;
using System.Collections.Generic;

namespace Callyzer.App.Models.Api
{
    public class SyncPayloadModel
    {
        public List<CallLogSyncModel> CallLogs { get; set; } = new();
    }

    public class CallLogSyncModel
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string CallType { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationInSeconds { get; set; }
        public string SimId { get; set; } = string.Empty;
    }
}
