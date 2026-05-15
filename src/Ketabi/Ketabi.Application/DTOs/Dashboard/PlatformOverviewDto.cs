using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.AuditLogs;

namespace Ketabi.Application.DTOs.Dashboard
{
    public class PlatformOverviewDto
    {
        public int NumberOfUsers { get; set; }
        public int NumberOfListings { get; set; }
        public int NumberOfTrades { get; set; }
        public int NumberOfActiveTrades { get; set; }
        public int NumberOfPendingListings { get; set; }


        public int NumberOfUserThisMonth { get; set; }
        public int NumberOfTradesThisWeek { get; set; }

        public double PlatformHealth { get; set; } = 98.4; // Placeholder
        public int HighPriorityReportsCount { get; set; }

        public IEnumerable<UserGrowthDto> UserGrowth { get; set; }
        public IEnumerable<CategoryDistributionDto> CategoryDistribution { get; set; }
        public IEnumerable<AuditLogDto> RecentAuditLogs { get; set; }

        public PlatformOverviewDto()
        {
            CategoryDistribution = new List<CategoryDistributionDto>();
            UserGrowth = new List<UserGrowthDto>();
            RecentAuditLogs = new List<AuditLogDto>();

        }

    }
}
