using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.DTOs.Dashboard;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services
{
    internal class DashboardService:IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PlatformOverviewDto> GetPlatformOverviewAsync()
        {
            var pOverview=new PlatformOverviewDto();
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            pOverview.NumberOfUsers=await _unitOfWork.Users.CountAsync();
            pOverview.NumberOfListings = await _unitOfWork.Listings.CountAsync(); // replace later with overloaded CountAsync to include only approved listings
            pOverview.NumberOfTrades = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Completed);
            pOverview.NumberOfActiveTrades = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Approved);
            //pOverview.NumberOfPendingListings later after the entity is updated

            pOverview.NumberOfUserThisMonth = await _unitOfWork.Users.CountAsync(u => u.CreatedAt >= startOfMonth);
            pOverview.NumberOfTradesThisWeek = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Completed && r.CreatedAt >= now.AddDays(-7));


            pOverview.UserGrowth = await GetUserGrowthAsync(now);
            pOverview.CategoryDistribution = await GetCategoryDistributionAsync();


            return pOverview;
        }

        private async Task<IReadOnlyList<UserGrowthDto>> GetUserGrowthAsync(DateTime now)
        {
            var firstMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);
            var growth = new List<UserGrowthDto>();

            for (var i = 0; i < 12; i++)
            {
                var monthStart = firstMonth.AddMonths(i);
                var nextMonthStart = monthStart.AddMonths(1);

                growth.Add(new UserGrowthDto
                {
                    Month = DateOnly.FromDateTime(monthStart),
                    NumberOfUsers = await _unitOfWork.Users.CountAsync(u =>
                        u.CreatedAt >= monthStart && u.CreatedAt < nextMonthStart),
                    NumberOfBooks = await _unitOfWork.Listings.CountAsync(b =>
                        b.CreatedAt >= monthStart && b.CreatedAt < nextMonthStart)
                });
            }

            return growth;
        }

        private async Task<IReadOnlyList<CategoryDistributionDto>> GetCategoryDistributionAsync()
        {
            var categories = await _unitOfWork.Categories.GetTopCategoryListingCountsAsync(10);

            return categories
                .Select(c => new CategoryDistributionDto
                {
                    Name = c.Name,
                    Value = c.ListingCount
                })
                .ToList();
        }

    }
}
