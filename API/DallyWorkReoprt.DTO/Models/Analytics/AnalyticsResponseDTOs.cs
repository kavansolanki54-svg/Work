using System;

namespace DallyWorkReoprt.DTO.Models.Analytics
{
    public class AnalyticsSummaryDTO
    {
        public int TotalCalls { get; set; }
        public int TotalDuration { get; set; } // in seconds
        public int UniqueContacts { get; set; }
        public double AnswerRate { get; set; }
    }

    public class TopContactDTO
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public int TotalCalls { get; set; }
        public int TotalDuration { get; set; }
        public int Rank { get; set; }
    }

    public class DailyTrendDTO
    {
        public DateTime Date { get; set; }
        public int IncomingCalls { get; set; }
        public int OutgoingCalls { get; set; }
        public int MissedCalls { get; set; }
        public int RejectedCalls { get; set; }
    }
}
