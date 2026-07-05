using System;
using System.Collections.Generic;

namespace DallyWorkReoprt.DTO.Models
{
    public class DashboardDTO
    {
        public int TotalEmployees { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalClients { get; set; }
        public int ReportsPending { get; set; }
        public List<RecentActivityDTO> RecentActivities { get; set; } = new List<RecentActivityDTO>();
    }

    public class RecentActivityDTO
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Type { get; set; }
    }
}
