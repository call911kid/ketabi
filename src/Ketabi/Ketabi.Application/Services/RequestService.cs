using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Queries;
using Ketabi.Application.DTOs.Requests;
using Ketabi.Application.Exceptions;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Interfaces;
using Ketabi.Application.DTOs.Notifications;

namespace Ketabi.Application.Services;

internal class RequestService : IRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public RequestService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<RequestDto> CreateBorrowRequestAsync(Guid requesterId, CreateBorrowRequestDto dto)
    {
        var listing = await _unitOfWork.Listings.GetByIdAsync(dto.ListingId);

        if (listing is null)
            throw new NotFoundException("Listing not found.");

        if (!listing.IsAvailable)
            throw new ConflictException("This listing is not available.");

        if (listing.UserId == requesterId)
            throw new ForbiddenException("You cannot request your own listing.");

        if (listing.SharingMode is not SharingMode.Borrow and not SharingMode.Both)
            throw new BadRequestException("This listing is not available for borrowing.");

        if (dto.ReturnDate <= DateTime.UtcNow)
            throw new BadRequestException("Return date must be in the future.");

        if (await _unitOfWork.Requests.HasActiveRequestForListingAsync(listing.Id))
            throw new ConflictException("This listing already has an active request.");

        var request = new Request
        {
            Id = Guid.NewGuid(),
            Type = RequestType.Borrow,
            Status = RequestStatus.Pending,
            RequestDate = DateTime.UtcNow,
            SenderId = requesterId,
            ReceiverId = listing.UserId,
            ListingId = listing.Id,
            ReturnDate = dto.ReturnDate,
            Note = dto.Note
        };

        await _unitOfWork.Requests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(requesterId);
            var senderFullName = GetFullName(sender);
            var bookTitle = listing.Title ?? "a book";

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = request.ReceiverId,
                Title = "New Borrow Request",
                Content = $"{Truncate(senderFullName, 60)} wants to borrow \"{Truncate(bookTitle, 80)}\"",
                NotificationType = NotificationType.RequestUpdate
            });
        }
        catch { }

        return await GetExistingRequestDtoAsync(request.Id);
    }

    public async Task<RequestDto> CreateExchangeRequestAsync(Guid requesterId, CreateExchangeRequestDto dto)
    {
        var listing = await _unitOfWork.Listings.GetByIdAsync(dto.ListingId);

        if (listing is null)
            throw new NotFoundException("Listing not found.");

        if (!listing.IsAvailable)
            throw new ConflictException("This listing is not available.");

        if (listing.UserId == requesterId)
            throw new ForbiddenException("You cannot request your own listing.");

        if (listing.SharingMode is not SharingMode.Exchange and not SharingMode.Both)
            throw new BadRequestException("This listing is not available for exchange.");

        var offeredListing = await _unitOfWork.Listings.GetByIdAsync(dto.OfferedListingId);

        if (offeredListing is null)
            throw new NotFoundException("Offered listing not found.");

        if (!offeredListing.IsAvailable)
            throw new ConflictException("The offered listing is not available.");

        if (offeredListing.UserId != requesterId)
            throw new ForbiddenException("You can only offer one of your own listings.");

        if (offeredListing.Id == listing.Id)
            throw new BadRequestException("You cannot offer the requested listing.");

        if (await _unitOfWork.Requests.HasActiveRequestForListingAsync(listing.Id))
            throw new ConflictException("This listing already has an active request.");

        if (await _unitOfWork.Requests.HasActiveRequestForListingAsync(offeredListing.Id))
            throw new ConflictException("The offered listing already has an active request.");

        var request = new Request
        {
            Id = Guid.NewGuid(),
            Type = RequestType.Exchange,
            Status = RequestStatus.Pending,
            RequestDate = DateTime.UtcNow,
            SenderId = requesterId,
            ReceiverId = listing.UserId,
            ListingId = listing.Id,
            OfferedListingId = offeredListing.Id,
            Note = dto.Note
        };

        await _unitOfWork.Requests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(requesterId);
            var senderFullName = GetFullName(sender);
            var bookTitle = listing.Title ?? "a book";

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = request.ReceiverId,
                Title = "New Exchange Request",
                Content = $"{Truncate(senderFullName, 60)} wants to exchange for \"{Truncate(bookTitle, 80)}\"",
                NotificationType = NotificationType.RequestUpdate
            });
        }
        catch { }

        return await GetExistingRequestDtoAsync(request.Id);
    }

    public async Task<PagedResponseDto<RequestDto>> GetIncomingRequestsAsync(Guid ownerId, RequestQueryDto query)
    {
        var paged = await _unitOfWork.Requests.GetIncomingDetailsAsync(ownerId, query.Status, query.Page, query.PageSize);

        return new PagedResponseDto<RequestDto>
        {
            Items = paged.Items.Select(MapToDto).ToList(),
            TotalCount = paged.TotalCount,
            Page = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<PagedResponseDto<RequestDto>> GetOutgoingRequestsAsync(Guid requesterId, RequestQueryDto query)
    {
        var paged = await _unitOfWork.Requests.GetOutgoingDetailsAsync(requesterId, query.Status, query.Page, query.PageSize);

        return new PagedResponseDto<RequestDto>
        {
            Items = paged.Items.Select(MapToDto).ToList(),
            TotalCount = paged.TotalCount,
            Page = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public async Task<RequestDto> GetRequestByIdAsync(Guid requestId, Guid userId)
    {
        var request = await _unitOfWork.Requests.GetDetailsAsync(requestId);

        if (request is null)
            throw new NotFoundException("Request not found.");

        if (request.SenderId != userId && request.ReceiverId != userId)
            throw new ForbiddenException("You can only view requests you participate in.");

        return MapToDto(request);
    }

    public async Task<RequestDto> UpdateRequestStatusAsync(Guid ownerId, Guid requestId, UpdateRequestStatusDto dto)
    {
        var request = await _unitOfWork.Requests.GetDetailsAsync(requestId);

        if (request is null)
            throw new NotFoundException("Request not found.");

        if (request.ReceiverId != ownerId)
            throw new ForbiddenException("Only the listing owner can update this request.");

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException("Only pending requests can be updated.");

        if (dto.Status is not RequestStatus.Approved and not RequestStatus.Rejected)
            throw new BadRequestException("Request can only be approved or rejected.");

        request.Status = dto.Status;

        if (!string.IsNullOrWhiteSpace(dto.Note))
            request.Note = dto.Note;

        if (dto.Status == RequestStatus.Approved)
        {
            if (request.Listing is null)
                throw new NotFoundException("Listing not found.");

            request.Listing.IsAvailable = false;

            if (request.OfferedListing is not null)
                request.OfferedListing.IsAvailable = false;

            var otherPendingRequests = await _unitOfWork.Requests.GetPendingRequestsForListingAsync(request.ListingId, request.Id);

            foreach (var pendingRequest in otherPendingRequests)
            {
                pendingRequest.Status = RequestStatus.Rejected;
                _unitOfWork.Requests.Update(pendingRequest);
            }

            if (request.OfferedListingId.HasValue)
            {
                var offeredListingRequests = await _unitOfWork.Requests.GetPendingRequestsForListingAsync(request.OfferedListingId.Value, request.Id);

                foreach (var pendingRequest in offeredListingRequests)
                {
                    pendingRequest.Status = RequestStatus.Rejected;
                    _unitOfWork.Requests.Update(pendingRequest);
                }
            }
        }

        _unitOfWork.Requests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var bookTitle = request.Listing?.Title ?? "the book";
            var owner = await _unitOfWork.Users.GetByIdAsync(ownerId);
            var ownerName = GetFullName(owner);

            if (dto.Status == RequestStatus.Approved)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserId = request.SenderId,
                    Title = "Request Approved 🎉",
                    Content = $"Your request for \"{Truncate(bookTitle, 80)}\" has been approved. Coordinate with {Truncate(ownerName, 60)} to arrange the handoff.",
                    NotificationType = NotificationType.RequestUpdate
                });
            }
            else if (dto.Status == RequestStatus.Rejected)
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDto
                {
                    UserId = request.SenderId,
                    Title = "Request Not Approved",
                    Content = $"Your request for \"{Truncate(bookTitle, 80)}\" was not approved this time.",
                    NotificationType = NotificationType.RequestUpdate
                });
            }
        }
        catch { }

        return await GetExistingRequestDtoAsync(request.Id);
    }

    public async Task<RequestDto> CancelRequestAsync(Guid requesterId, Guid requestId)
    {
        var request = await _unitOfWork.Requests.GetDetailsAsync(requestId);

        if (request is null)
            throw new NotFoundException("Request not found.");

        if (request.SenderId != requesterId)
            throw new ForbiddenException("Only the requester can cancel this request.");

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException("Only pending requests can be cancelled.");

        request.Status = RequestStatus.Cancelled;

        _unitOfWork.Requests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return await GetExistingRequestDtoAsync(request.Id);
    }

    public async Task<RequestDto> CompleteRequestAsync(Guid userId, Guid requestId)
    {
        var request = await _unitOfWork.Requests.GetDetailsAsync(requestId);

        if (request is null)
            throw new NotFoundException("Request not found.");

        if (request.SenderId != userId && request.ReceiverId != userId)
            throw new ForbiddenException("You can only complete requests you participate in.");

        if (request.Status != RequestStatus.Approved)
            throw new ConflictException("Only approved requests can be completed.");

        request.Status = RequestStatus.Completed;

        _unitOfWork.Requests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            var bookTitle = request.Listing?.Title ?? "the book";

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = request.SenderId,
                Title = "Trade Completed ✅",
                Content = $"Your trade for \"{Truncate(bookTitle, 80)}\" is marked as complete. You can now leave a review.",
                NotificationType = NotificationType.RequestUpdate
            });

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = request.ReceiverId,
                Title = "Trade Completed ✅",
                Content = $"Your trade for \"{Truncate(bookTitle, 80)}\" is marked as complete. You can now leave a review.",
                NotificationType = NotificationType.RequestUpdate
            });
        }
        catch { }

        return await GetExistingRequestDtoAsync(request.Id);
    }

    private async Task<RequestDto> GetExistingRequestDtoAsync(Guid requestId)
    {
        var request = await _unitOfWork.Requests.GetDetailsAsync(requestId);

        if (request is null)
            throw new NotFoundException("Request not found.");

        return MapToDto(request);
    }

    private RequestDto MapToDto(Request request)
    {
        var listing = request.Listing;
        var requester = request.Sender;
        var owner = request.Receiver;
        var offeredListing = request.OfferedListing;

        return new RequestDto
        {
            RequestId = request.Id,
            Type = request.Type,
            Status = request.Status,
            RequestDate = request.RequestDate,
            Note = request.Note,
            ReturnDate = request.ReturnDate,
            ListingId = request.ListingId,
            ListingTitle = listing?.Title ?? string.Empty,
            ListingAuthor = listing?.Author ?? string.Empty,
            ListingImageUrl = listing?.ImageUrl,
            ListingCategory = listing?.Category?.Name ?? string.Empty,
            ListingCondition = listing?.Condition ?? default,
            ListingSharingMode = listing?.SharingMode ?? default,
            RequesterId = request.SenderId,
            RequesterFullName = GetFullName(requester),
            RequesterUserName = GetUserName(requester),
            RequesterAvatarUrl = requester?.ProfilePictureUrl ?? string.Empty,
            RequesterLocation = GetLocation(requester),
            RequesterReputationScore = requester?.ReputationScore ?? 0,
            RequesterReviewCount = 0,
            RequesterTradesCount = 0,
            OwnerId = request.ReceiverId,
            OwnerFullName = GetFullName(owner),
            OwnerUserName = GetUserName(owner),
            OwnerAvatarUrl = owner?.ProfilePictureUrl ?? string.Empty,
            OwnerLocation = GetLocation(owner),
            OwnerReputationScore = owner?.ReputationScore ?? 0,
            OwnerReviewCount = 0,
            OwnerTradesCount = 0,
            OfferedListingId = request.OfferedListingId,
            OfferedListingTitle = offeredListing?.Title,
            OfferedListingAuthor = offeredListing?.Author,
            OfferedListingImageUrl = offeredListing?.ImageUrl,
            OfferedListingCategory = offeredListing?.Category?.Name,
            OfferedListingCondition = offeredListing?.Condition,
            OfferedListingSharingMode = offeredListing?.SharingMode
        };
    }

    private string GetFullName(User? user)
    {
        if (user is null)
            return string.Empty;

        return $"{user.FirstName} {user.LastName}".Trim();
    }

    private string GetUserName(User? user)
    {
        if (string.IsNullOrWhiteSpace(user?.Email))
            return string.Empty;

        return user.Email.Split('@')[0];
    }

    private string GetLocation(User? user)
    {
        if (user is null)
            return string.Empty;

        if (!string.IsNullOrWhiteSpace(user.City) && !string.IsNullOrWhiteSpace(user.Governorate))
            return $"{user.City}, {user.Governorate}";

        return user.City ?? user.Governorate ?? string.Empty;
    }

    private static string Truncate(string? value, int maxLength)
        => string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Length <= maxLength ? value : value[..maxLength];
}
