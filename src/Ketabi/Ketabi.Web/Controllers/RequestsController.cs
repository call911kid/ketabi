using AutoMapper;
using Ketabi.Application.DTOs.Queries;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Requests;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ketabi.Web.Controllers;

[Authorize]
public class RequestsController : BaseController
{
    private readonly IRequestService _requestService;
    private readonly IMapper _mapper;

    public RequestsController(IRequestService requestService, IMapper mapper)
    {
        _requestService = requestService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tab = "incoming")
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var query = new RequestQueryDto
        {
            Status = null, // Get all statuses
            Page = 1,
            PageSize = 50 // Load more for the index page
        };

        var incomingRequests = await _requestService.GetIncomingRequestsAsync(userId, query);
        var outgoingRequests = await _requestService.GetOutgoingRequestsAsync(userId, query);

        var viewModel = new RequestsIndexViewModel
        {
            ActiveTab = tab,
            IncomingRequests = incomingRequests.Items.Select(r => MapToCardViewModel(r, userId)).ToList(),
            OutgoingRequests = outgoingRequests.Items.Select(r => MapToCardViewModel(r, userId)).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid requestId, string status, string? note)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated." });
        }

        if (status.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
        {
            status = RequestStatus.Approved.ToString();
        }

        if (!Enum.TryParse<RequestStatus>(status, true, out var parsedStatus))
        {
            return Json(new { success = false, message = "Invalid request status." });
        }

        try
        {
            var dto = new Ketabi.Application.DTOs.Requests.UpdateRequestStatusDto
            {
                Status = parsedStatus,
                Note = note
            };

            await _requestService.UpdateRequestStatusAsync(userId, requestId, dto);

            return Json(new { success = true, message = $"Request {parsedStatus.ToString().ToLower()} successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelRequest(Guid requestId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated." });
        }

        try
        {
            await _requestService.CancelRequestAsync(userId, requestId);

            return Json(new { success = true, message = "Request cancelled successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteRequest(Guid requestId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Json(new { success = false, message = "User not authenticated." });
        }

        try
        {
            await _requestService.CompleteRequestAsync(userId, requestId);

            return Json(new { success = true, message = "Request completed successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private RequestCardViewModel MapToCardViewModel(Ketabi.Application.DTOs.Requests.RequestDto dto, Guid currentUserId)
    {
        var isIncoming = dto.OwnerId == currentUserId;

        var viewModel = new RequestCardViewModel
        {
            RequestId = dto.RequestId,
            Status = dto.Status,
            RequestDate = dto.RequestDate,
            Note = dto.Note,
            BookId = dto.ListingId,
            BookTitle = dto.ListingTitle,
            BookImageUrl = dto.ListingImageUrl ?? string.Empty,
            BookCategory = dto.ListingCategory,
            IsBorrow = dto.Type == RequestType.Borrow,
            ReturnDate = dto.ReturnDate,
            Requester = new UserSummaryViewModel
            {
                UserId = dto.RequesterId,
                FullName = dto.RequesterFullName,
                UserName = dto.RequesterUserName,
                AvatarUrl = dto.RequesterAvatarUrl,
                Location = dto.RequesterLocation,
                ReputationScore = dto.RequesterReputationScore,
                ReviewCount = dto.RequesterReviewCount,
                TradesCount = dto.RequesterTradesCount
            },
            Owner = new UserSummaryViewModel
            {
                UserId = dto.OwnerId,
                FullName = dto.OwnerFullName,
                UserName = dto.OwnerUserName,
                AvatarUrl = dto.OwnerAvatarUrl,
                Location = dto.OwnerLocation,
                ReputationScore = dto.OwnerReputationScore,
                ReviewCount = dto.OwnerReviewCount,
                TradesCount = dto.OwnerTradesCount
            },
            CanAcceptOrReject = isIncoming && dto.Status == RequestStatus.Pending,
            CanWithdraw = !isIncoming && dto.Status == RequestStatus.Pending
        };

        if (dto.OfferedListingId.HasValue)
        {
            viewModel.OfferedBook = new BookCardViewModel
            {
                BookId = dto.OfferedListingId.Value,
                Title = dto.OfferedListingTitle ?? string.Empty,
                Author = dto.OfferedListingAuthor ?? string.Empty,
                ImageUrl = dto.OfferedListingImageUrl ?? string.Empty,
                Category = dto.OfferedListingCategory ?? string.Empty,
                Condition = dto.OfferedListingCondition ?? default,
                SharingMode = dto.OfferedListingSharingMode ?? default,
                IsAvailable = true
            };
        }

        return viewModel;
    }
}