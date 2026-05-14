using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Application.DTOs.Dashboard
{
    public class PlatformOverviewDto
    {
        public int NumberOfUsers { get; init; }
        public int NumberOfListings { get; init; }
        public int NumberOfTrades { get; init; }
        public int NumberOfActiveTrades { get; init; }
        public int NumberOfPendingListings { get; init; }


        public int NumberOfUserThisMonth { get; init; }
        public int NumberOfTradesThisWeek { get; init; }

        public IEnumerable<UserGrowthDto> UserGrowth { get; init; }
        public IEnumerable<CategoryDistributionDto> CategoryDistribution { get; init; }

        public PlatformOverviewDto()
        {
            CategoryDistribution = new List<CategoryDistributionDto>();
            UserGrowth = new List<UserGrowthDto>();

        }

    }
}
