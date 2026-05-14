using System;
using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Dashboard;

namespace Ketabi.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<PlatformOverviewDto> GetPlatformOverviewAsync();
        Task<UserOverviewDto> GetUserOverviewAsync(PagedRequestDto pagination);
        Task<RequestsOverviewDto> GetRequestsOverviewAsync(PagedRequestDto pagination);

    }
}
