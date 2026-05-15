using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.AuditLogs;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Dashboard;
using Ketabi.Application.DTOs.Requests;
using Ketabi.Application.DTOs.Users;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services
{
    internal class DashboardService : IDashboardService
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
            pOverview.RecentAuditLogs = await GetRecentAuditLogsAsync();
            pOverview.HighPriorityReportsCount = await GetHighPriorityReportsCountAsync();

            return pOverview;
        }

        public async Task<BookModerationDto> GetBookModerationAsync()
        {
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);

            var moderationDto = new BookModerationDto
            {
                TotalPendingBooks = await _unitOfWork.Listings.CountAsync(l => l.ListingStatus == ListingStatus.Pending),
                TotalApprovedBooks = await _unitOfWork.Listings.CountAsync(l => l.ListingStatus == ListingStatus.Approved),
                TotalRejectedBooks = await _unitOfWork.Listings.CountAsync(l => l.ListingStatus == ListingStatus.Rejected),
                BooksApprovedThisWeek = await _unitOfWork.Listings.CountAsync(l => l.ListingStatus == ListingStatus.Approved && l.UpdatedAt >= weekAgo),
                BooksRejectedThisWeek = await _unitOfWork.Listings.CountAsync(l => l.ListingStatus == ListingStatus.Rejected && l.UpdatedAt >= weekAgo),
                PendingBooks = await GetPendingBooksAsync()
            };

            return moderationDto;
        }

        public async Task<UserOverviewDto> GetUserOverviewAsync(PagedRequestDto pagination, string? search = null)
        {
            //n+1 :(
            var pagedUsers = await _unitOfWork.Users.GetPagedAsync(pagination.Page, pagination.PageSize, search);
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

        public async Task<RequestsOverviewDto> GetRequestsOverviewAsync(PagedRequestDto pagination)
        {
            var pagedRequests=await _unitOfWork.Requests.GetPagedAsync(pagination.Page, pagination.PageSize);

            var requests = pagedRequests.Items.Select(r => new RequestSummaryDto
            {
                RequestId = r.Id,
                Type = r.Type,
                Status = r.Status,
                RequestDate = r.RequestDate,

                ListingId = r.ListingId,
                ListingTitle = r.Listing.Title,
                ListingAuthor = r.Listing.Author,
                ListingImageUrl = r.Listing.ImageUrl,

                RequesterId = r.SenderId,
                RequesterFullName = r.Sender.FirstName + ' ' + r.Sender.LastName,
                RequsterEmail = r.Sender.Email,


                OwnerId = r.Receiver.Id,
                OwnerFullName = r.Receiver.FirstName + ' ' + r.Receiver.LastName,
                OwnerEmail = r.Receiver.Email

            });

            return new RequestsOverviewDto
            {
                NumberOfPendingRequests = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Pending),
                NumberOfApprovedRequests = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Approved),
                NumberOfCompletedRequest = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Completed),
                NumberOfRejectedRequests = await _unitOfWork.Requests.CountAsync(r => r.Status == RequestStatus.Rejected),

                Requests=requests
                
            };
        }

        private async Task<IReadOnlyList<UserGrowthDto>> GetUserGrowthAsync(DateTime now)
        {
            var firstMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-6); // Last 7 months
            var growth = new List<UserGrowthDto>();

            for (var i = 0; i < 7; i++)
            {
                var monthStart = firstMonth.AddMonths(i);
                var nextMonthStart = monthStart.AddMonths(1);

                growth.Add(new UserGrowthDto
                {
                    Month = monthStart.ToString("MMM"),
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

            var colors = new[] { "#6366F1", "#F59E0B", "#10B981", "#3B82F6", "#EC4899", "#8B5CF6", "#06B6D4", "#F97316", "#84CC16", "#EF4444" };

            return categories
                .Select((c, index) => new CategoryDistributionDto
                {
                    Name = c.Name,
                    Value = c.ListingCount,
                    Color = colors[index % colors.Length]
                })
                .ToList();
        }

        private async Task<IReadOnlyList<AuditLogDto>> GetRecentAuditLogsAsync()
        {
            // For now, return mock data. Replace with actual audit logs when implemented.
            return new List<AuditLogDto>
            {
                new AuditLogDto { Id = "al1", AdminName = "Layla Hassan", Action = "Banned User", Target = "Chen Wei", TargetType = "user", Details = "Account permanently banned after 3 harassment reports within 30 days.", Timestamp = DateTime.UtcNow.AddHours(-1), Severity = "critical", Ip = "196.52.100.4" },
                new AuditLogDto { Id = "al2", AdminName = "Omar Khalil", Action = "Approved Book", Target = "1984 by George Orwell", TargetType = "book", Details = "Book listing approved after manual review. Condition verified.", Timestamp = DateTime.UtcNow.AddHours(-2), Severity = "info", Ip = "196.52.100.7" },
                new AuditLogDto { Id = "al3", AdminName = "Omar Khalil", Action = "Resolved Report", Target = "Report #r4", TargetType = "user", Details = "No-show report resolved. Warning issued to Zara Al-Farsi.", Timestamp = DateTime.UtcNow.AddHours(-3), Severity = "warning", Ip = "196.52.100.7" },
                new AuditLogDto { Id = "al4", AdminName = "Layla Hassan", Action = "Suspended User", Target = "Rajan Patel", TargetType = "user", Details = "Account suspended after 3 verified complaints about fake listings.", Timestamp = DateTime.UtcNow.AddDays(-1), Severity = "critical", Ip = "196.52.100.4" },
                new AuditLogDto { Id = "al5", AdminName = "Priya Sharma", Action = "Rejected Book", Target = "The Great Gatsby", TargetType = "book", Details = "Listing rejected: book condition (Worn) requires photo documentation.", Timestamp = DateTime.UtcNow.AddDays(-1).AddHours(1), Severity = "warning", Ip = "196.52.100.9" },
                new AuditLogDto { Id = "al6", AdminName = "Layla Hassan", Action = "Added Category", Target = "Children", TargetType = "category", Details = "New category \"Children\" created with orange color tag.", Timestamp = DateTime.UtcNow.AddDays(-2), Severity = "info", Ip = "196.52.100.4" },
                new AuditLogDto { Id = "al7", AdminName = "Layla Hassan", Action = "Broadcast Notification", Target = "All Users", TargetType = "notification", Details = "Maintenance window announcement sent to 847 users.", Timestamp = DateTime.UtcNow.AddDays(-3), Severity = "info", Ip = "196.52.100.4" },
            };
        }

        private async Task<int> GetHighPriorityReportsCountAsync()
        {
            // For now, return mock data. Replace with actual reports when implemented.
            return 2;
        }

        private async Task<IReadOnlyList<PendingBookDto>> GetPendingBooksAsync()
        {
            // Get pending listings with owner information
            var pendingListings = await _unitOfWork.Listings.FindWithIncludesAsync(l => l.ListingStatus == ListingStatus.Pending);

            return pendingListings.Take(20).Select(l => new PendingBookDto
            {
                Id = l.Id.ToString(),
                Title = l.Title,
                Author = l.Author,
                Category = l.Category.Name,
                Condition = l.Condition.ToString(),
                OwnerId = l.UserId.ToString(),
                OwnerName = $"{l.User.FirstName} {l.User.LastName}",
                SubmittedAt = l.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                Status = l.ListingStatus?.ToString() ?? "Unknown",
                Description = l.Description ?? "",
                TransactionType = l.SharingMode.ToString(),
                CoverColor = "#6366F1", // Default color, could be randomized or based on category
                ISBN = l.ISBN,
                Language = l.Language,
                Publisher = l.Publisher,
                ImageUrl = l.ImageUrl,
                LocationNote = l.LocationNote,
                SharingDurationInDays = l.SharingDurationInDays,
                IsAvailable = l.IsAvailable,
                Tags = l.Tags.Select(t => t.Tag).ToList()
            }).ToList();
        }

    }
}
