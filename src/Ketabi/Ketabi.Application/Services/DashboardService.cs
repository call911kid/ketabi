using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Dashboard;
using Ketabi.Application.DTOs.Users;
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

        public async Task<UserOverviewDto> GetUserOverviewAsync(PagedRequestDto pagination)
        {
            //n+1 :(
            var pagedUsers = await _unitOfWork.Users.GetPagedAsync(pagination.Page, pagination.PageSize);
            var users = new List<UserSummaryDto>();

            foreach (var user in pagedUsers.Items)
            {
                users.Add(new UserSummaryDto
                {
                    UserId = user.Id,
                    FullName = user.FirstName+' '+user.LastName,
                    UserName = user.Email.Split('@')[0],
                    AvatarUrl= user.ProfilePictureUrl,
                    Location = user.City,
                    ReputationScore = user.ReputationScore,
                    ReviewCount = await _unitOfWork.Reviews.CountReviewsForUserAsync(user.Id),
                    TradesCount = await _unitOfWork.Requests.CountCompletedTradesForUserAsync(user.Id),
                    ListingCount = await _unitOfWork.Listings.CountAsync(l => l.UserId == user.Id)
                });
            }

            return new UserOverviewDto
            {
                NumberOfUsers = pagedUsers.TotalCount,
                Users = new PagedResponseDto<UserSummaryDto>
                {
                    Items = users,
                    TotalCount = pagedUsers.TotalCount,
                    Page = pagedUsers.PageNumber,
                    PageSize = pagedUsers.PageSize
                }
            };
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
