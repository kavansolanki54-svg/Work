using System;
using System.Collections.Generic;

namespace DallyWorkReoprt.DTO.Models.Analytics
{
    public class SyncPayloadDTO
    {
        public List<CallLogSyncDTO> CallLogs { get; set; } = new List<CallLogSyncDTO>();
        public List<DailySummarySyncDTO> DailySummaries { get; set; } = new List<DailySummarySyncDTO>();
    }

    public class DailySummarySyncDTO
    {
        public DateTime Date { get; set; }
        public int TotalCalls { get; set; }
        public int TotalDuration { get; set; }
        public int IncomingCalls { get; set; }
        public int OutgoingCalls { get; set; }
        public int MissedCalls { get; set; }
    }

    public class CallLogSyncDTO
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string CallType { get; set; } = string.Empty; // Incoming, Outgoing, Missed, Rejected
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationInSeconds { get; set; }
        public string SimId { get; set; } = string.Empty;
    }
}
