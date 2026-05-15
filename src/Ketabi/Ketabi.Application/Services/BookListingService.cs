using System;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Users;
using Ketabi.Application.DTOs.Notifications;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Interfaces;
using static Ketabi.Application.Common.Messages;

namespace Ketabi.Application.Services
{
    internal class BookListingService : IBookListingService
    {
        private readonly IUnitOfWork _uow;
        private readonly AutoMapper.IMapper _mapper;
        private readonly INotificationService _notificationService;

        public BookListingService(IUnitOfWork uow, AutoMapper.IMapper mapper, INotificationService notificationService)
        {
            _uow = uow;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<BookDetailDto> CreateBookAsync(CreateBookDto createDto, Guid userId)
        {
            var book = new BookListing
            {
                Id = Guid.NewGuid(),
                Title = createDto.Title,
                Author = createDto.Author,
                ISBN = createDto.ISBN,
                Description = createDto.Description,
                Language = createDto.Language,
                Publisher = createDto.Publisher,
                CategoryId = createDto.CategoryId,
                Condition = createDto.Condition,
                SharingMode = createDto.SharingMode,
                SharingDurationInDays = createDto.SharingDurationInDays,
                ImageUrl = createDto.ImageUrl,
                LocationNote = createDto.LocationNote,
                UserId = userId,
                ListingStatus=ListingStatus.Pending
            };

            if (createDto.Tags != null && createDto.Tags.Any())
            {
                book.Tags = createDto.Tags.Select(t => new BookListingTag { Id = Guid.NewGuid(), Tag = t }).ToList();
            }

            await _uow.Listings.AddAsync(book);
            await _uow.SaveChangesAsync();

            return new BookDetailDto
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Language = book.Language,
                Publisher = book.Publisher,
                CategoryId = book.CategoryId,
                Category = (await _uow.Categories.GetByIdAsync(book.CategoryId))?.Name ?? string.Empty,
                Condition = book.Condition,
                SharingMode = book.SharingMode,
                SharingDurationInDays = book.SharingDurationInDays,
                IsAvailable = book.IsAvailable,
                ImageUrl = book.ImageUrl ?? string.Empty,
                ImageUrls = book.Tags.Select(t => t.Tag).ToList(),
                Tags = book.Tags.Select(t => t.Tag).ToList(),
                LocationNote = book.LocationNote,
                ListedAt = book.CreatedAt,
                DistanceInKm = 0,
                Owner = new UserSummaryDto { UserId = book.UserId }
            };
        }

        public async Task DeleteBookAsync(Guid bookId, Guid userId)
        {
            var book = await _uow.Listings.GetByIdAsync(bookId);
            if (book == null) throw new KeyNotFoundException("Book not found");
            if (book.UserId != userId) throw new UnauthorizedAccessException();

            book.IsDeleted = true;
            book.DeletedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookSummaryDto>> GetAllBooksAsync(int pageNumber, int pageSize)
        {
            var paged = await _uow.Listings.FindPagedWithIncludesAsync(l => l.ListingStatus == ListingStatus.Approved, pageNumber, pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<BookSummaryDto>> GetBooksByAuthorAsync(string author, int pageNumber, int pageSize)
        {
            var paged = await _uow.Listings.FindPagedWithIncludesAsync(b => b.Author.Contains(author) && b.ListingStatus == ListingStatus.Approved, pageNumber, pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<BookSummaryDto>> GetBooksByCategoryAsync(string category, int pageNumber, int pageSize)
        {
            var paged = await _uow.Listings.FindPagedWithIncludesAsync(b => b.Category != null && b.Category.Name == category && b.ListingStatus == ListingStatus.Approved, pageNumber, pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<BookSummaryDto>> GetRelatedBooksAsync(Guid bookId, int pageNumber, int pageSize)
        {
            var book = await _uow.Listings.GetByIdAsync(bookId);
            if (book == null) return Enumerable.Empty<BookSummaryDto>();

            var paged = await _uow.Listings.FindPagedWithIncludesAsync(b => b.CategoryId == book.CategoryId && b.Id != bookId && b.ListingStatus == ListingStatus.Approved, pageNumber, pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<BookSummaryDto>> GetBooksByUserIdAsync(Guid userId, int pageNumber, int pageSize)
        {
            var paged = await _uow.Listings.FindPagedWithIncludesAsync(b => b.UserId == userId, pageNumber, pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query, int pageNumber, int pageSize)
        {
            var normalizedQuery = query?.Trim().ToLowerInvariant() ?? string.Empty;
            var paged = await _uow.Listings.FindPagedWithIncludesAsync(b =>
                (b.Title != null && b.Title.ToLowerInvariant().Contains(normalizedQuery)) ||
                (b.Author != null && b.Author.ToLowerInvariant().Contains(normalizedQuery)) ||
                (b.ISBN != null && b.ISBN.ToLowerInvariant().Contains(normalizedQuery)) &&
                b.ListingStatus == ListingStatus.Approved,
                pageNumber,
                pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<BookSummaryDto>> GetFilteredBooksAsync(BookFilterDto filter)
        {
            var listings = await _uow.Listings.GetAllWithIncludesAsync();
            IEnumerable<BookListing> query = listings.Where(l => l.ListingStatus == ListingStatus.Approved);

            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                var normalizedQuery = filter.Query.Trim().ToLowerInvariant();
                query = query.Where(b =>
                    (b.Title != null && b.Title.ToLowerInvariant().Contains(normalizedQuery)) ||
                    (b.Author != null && b.Author.ToLowerInvariant().Contains(normalizedQuery)) ||
                    (b.ISBN != null && b.ISBN.ToLowerInvariant().Contains(normalizedQuery)) ||
                    (b.Description != null && b.Description.ToLowerInvariant().Contains(normalizedQuery)));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(b => b.CategoryId == filter.CategoryId.Value);

            if (filter.Tags != null && filter.Tags.Any())
                query = query.Where(b => b.Tags.Any(t => filter.Tags.Contains(t.Tag)));

            if (filter.Condition.HasValue)
                query = query.Where(b => b.Condition == filter.Condition.Value);

            if (filter.SharingMode.HasValue)
                query = query.Where(b => b.SharingMode == filter.SharingMode.Value);

            if (filter.IsAvailable.HasValue)
                query = query.Where(b => b.IsAvailable == filter.IsAvailable.Value);

            if (filter.OwnerId.HasValue)
                query = query.Where(b => b.UserId == filter.OwnerId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                if (filter.SortBy == "listedAt")
                    query = filter.SortDescending ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt);
            }

            var items = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<BookDetailDto> GetBookByIdAsync(Guid bookId, Guid? userId = null)
        {
            var listing = await _uow.Listings.GetByIdWithIncludesAsync(bookId);
            if (listing == null) throw new KeyNotFoundException("Book not found");

            // Check if user can view this book
            if (listing.ListingStatus != ListingStatus.Approved &&
                (userId == null || listing.UserId != userId))
            {
                var canViewViaRequest = userId.HasValue &&
                    await _uow.Requests.IsUserPartyToRequestForListingAsync(userId.Value, bookId);

                if (!canViewViaRequest)
                    throw new UnauthorizedAccessException("Book not available");
            }

            var book = new BookDetailDto
            {
                BookId = listing.Id,
                Title = listing.Title,
                Author = listing.Author,
                ISBN = listing.ISBN,
                Description = listing.Description,
                Language = listing.Language,
                Publisher = listing.Publisher,
                CategoryId = listing.CategoryId,
                Category = listing.Category != null ? listing.Category.Name : string.Empty,
                Condition = listing.Condition,
                SharingMode = listing.SharingMode,
                SharingDurationInDays = listing.SharingDurationInDays,
                IsAvailable = listing.IsAvailable,
                ImageUrl = listing.ImageUrl ?? string.Empty,
                ImageUrls = listing.Tags.Select(t => t.Tag).ToList(),
                Tags = listing.Tags.Select(t => t.Tag).ToList(),
                LocationNote = listing.LocationNote,
                ListedAt = listing.CreatedAt,
                DistanceInKm = 0,
                Owner = new UserSummaryDto
                {
                    UserId = listing.User != null ? listing.User.Id : Guid.Empty,
                    FullName = listing.User != null ? $"{listing.User.FirstName} {listing.User.LastName}" : string.Empty,
                    UserName = listing.User != null ? (listing.User.Email?.Split('@')[0] ?? string.Empty) : string.Empty,
                    AvatarUrl = listing.User?.ProfilePictureUrl ?? AppConstants.DefaultProfilePic,
                    Location = listing.User != null ? $"{listing.User.City}, {listing.User.Governorate}" : string.Empty,
                    ReputationScore = listing.User != null ? listing.User.ReputationScore : 0,
                    ReviewCount = 0,
                    TradesCount = 0
                }
            };

            return book;
        }

        public async Task<UserSummaryDto> GetOwnerProfileAsync(Guid userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            return new UserSummaryDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.Email?.Split('@')[0] ?? string.Empty,
                AvatarUrl = user.ProfilePictureUrl ?? string.Empty,
                Location = $"{user.City}, {user.Governorate}",
                ReputationScore = user.ReputationScore,
                ReviewCount = 0,
                TradesCount = 0
            };
        }

        public async Task<BookDetailDto> UpdateBookAsync(Guid bookId, UpdateBookDto updateDto, Guid userId)
        {
            var book = await _uow.Listings.GetByIdAsync(bookId);
            if (book == null) throw new KeyNotFoundException("Book not found");
            if (book.UserId != userId) throw new UnauthorizedAccessException();

            if (updateDto.Title != null) book.Title = updateDto.Title;
            if (updateDto.Author != null) book.Author = updateDto.Author;
            if (updateDto.ISBN != null) book.ISBN = updateDto.ISBN;
            if (updateDto.Description != null) book.Description = updateDto.Description;
            if (updateDto.Language != null) book.Language = updateDto.Language;
            if (updateDto.Publisher != null) book.Publisher = updateDto.Publisher;
            if (updateDto.CategoryId.HasValue) book.CategoryId = updateDto.CategoryId.Value;
            if (updateDto.Condition.HasValue) book.Condition = updateDto.Condition.Value;
            if (updateDto.SharingMode.HasValue) book.SharingMode = updateDto.SharingMode.Value;
            if (updateDto.SharingDurationInDays.HasValue) book.SharingDurationInDays = updateDto.SharingDurationInDays.Value;
            if (updateDto.ImageUrl != null) book.ImageUrl = updateDto.ImageUrl;
            if (updateDto.LocationNote != null) book.LocationNote = updateDto.LocationNote;
            if (updateDto.IsAvailable.HasValue) book.IsAvailable = updateDto.IsAvailable.Value;

            await _uow.SaveChangesAsync();

            return await GetBookByIdAsync(bookId, userId);
        }

        public async Task ApproveListingAsync(Guid listingId) => await ChangeListingStatus(listingId, ListingStatus.Approved);
        
        public async Task RejectListingAsync(Guid listingId, string reasonForRejection)=> await ChangeListingStatus(listingId, ListingStatus.Rejected);
            
        public async Task RestoreToPendingAsync(Guid listingId) => await ChangeListingStatus(listingId, ListingStatus.Pending);
        
        public async Task<IEnumerable<BookSummaryDto>> GetListingsByStatusAsync(ListingStatus listingStatus, int pageNumber, int pageSize)
        {
            var paged = await _uow.Listings.FindPagedWithIncludesAsync(l=>l.ListingStatus==listingStatus, pageNumber,pageSize);
            return paged.Items.Select(b => _mapper.Map<BookSummaryDto>(b));
        }

        public async Task<IEnumerable<PendingBookDto>> GetUserPendingBooksAsync(Guid userId)
        {
            var pendingBooks = await _uow.Listings.FindWithIncludesAsync(l => l.UserId == userId && l.ListingStatus == ListingStatus.Pending);
            return pendingBooks.Select(b => new PendingBookDto
            {
                Id = b.Id.ToString(),
                Title = b.Title,
                Author = b.Author,
                Category = b.Category?.Name ?? string.Empty,
                Condition = b.Condition.ToString(),
                OwnerId = b.UserId.ToString(),
                OwnerName = $"{b.User?.FirstName ?? ""} {b.User?.LastName ?? ""}".Trim(),
                SubmittedAt = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                Status = b.ListingStatus.ToString(),
                RejectionReason = b.ReasonForRejection,
                Description = b.Description ?? string.Empty,
                TransactionType = b.SharingMode.ToString(),
                CoverColor = "#6366F1",
                ISBN = b.ISBN,
                Language = b.Language,
                Publisher = b.Publisher,
                ImageUrl = b.ImageUrl,
                LocationNote = b.LocationNote,
                SharingDurationInDays = b.SharingDurationInDays,
                IsAvailable = b.IsAvailable,
                Tags = b.Tags?.Select(t => t.Tag) ?? Enumerable.Empty<string>()
            });
        }

        private async Task ChangeListingStatus(Guid listingId, ListingStatus listingStatus, string reasonForRejection=null!)
        {
            var listing = await _uow.Listings.GetByIdAsync(listingId);
            if (listing == null) return;

            listing.ListingStatus = listingStatus;
            if(listingStatus==ListingStatus.Rejected||listingStatus== ListingStatus.Pending)
            {
                listing.ReasonForRejection = reasonForRejection;
            }

            await _uow.SaveChangesAsync();

            try
            {
                if (listingStatus == ListingStatus.Approved)
                {
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        UserId = listing.UserId,
                        Title = "Listing Approved",
                        Content = $"Your book \"{Truncate(listing.Title, 80)}\" is now live and visible to readers.",
                        NotificationType = NotificationType.System
                    });
                }
                else if (listingStatus == ListingStatus.Rejected)
                {
                    var reason = string.IsNullOrWhiteSpace(reasonForRejection)
                        ? "No reason provided."
                        : reasonForRejection;

                    await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                    {
                        UserId = listing.UserId,
                        Title = "Listing Not Approved",
                        Content = $"Your book \"{Truncate(listing.Title, 80)}\" was not approved. Reason: {Truncate(reason, 100)}",
                        NotificationType = NotificationType.System
                    });
                }
            }
            catch { }
        }

        private static string Truncate(string? value, int maxLength)
            => string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Length <= maxLength ? value : value[..maxLength];

    }
}
