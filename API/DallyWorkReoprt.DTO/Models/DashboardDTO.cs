namespace DallyWorkReoprt.DTO.Models
{
    public class DashboardDTO
    {
        public int TotalEmployees { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalClients { get; set; }
        public int TotalModules { get; set; }
        public List<RecentActivityDTO> RecentActivities { get; set; } = new List<RecentActivityDTO>();
        public List<MonthlyOverviewDTO> MonthlyOverview { get; set; } = new List<MonthlyOverviewDTO>();
        public List<StatusCountDTO> StatusWiseCounts { get; set; } = new List<StatusCountDTO>();
    }

    public class MonthlyOverviewDTO
    {
        public string Month { get; set; } = null!;
        public List<StatusCountDTO> StatusCounts { get; set; } = new List<StatusCountDTO>();
    }

    public class StatusCountDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public string StatusColor { get; set; } = null!;
        public int Count { get; set; }
    }

    public class RecentActivityDTO
    {
        public string Title { get; set; } = null!;
        public string Detail { get; set; } = null!;
        public DateTime ActivityDate { get; set; }
        public string Type { get; set; } = null!;
    }
}
