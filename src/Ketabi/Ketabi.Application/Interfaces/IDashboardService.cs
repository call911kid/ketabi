using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Dashboard;

namespace Ketabi.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<PlatformOverviewDto> GetPlatformOverviewAsync();

    }
}
