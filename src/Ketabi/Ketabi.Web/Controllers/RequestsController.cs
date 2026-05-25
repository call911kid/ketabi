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
    private readonly IBookListingService _bookListingService;
    private readonly IConversationService _conversationService;
    private readonly IMapper _mapper;

    public RequestsController(IRequestService requestService, IBookListingService bookListingService, IConversationService conversationService, IMapper mapper)
    {
        _requestService = requestService;
        _bookListingService = bookListingService;
        _conversationService = conversationService;
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
        var userPendingBooks = await _bookListingService.GetUserPendingBooksAsync(userId);

        // Build initial viewmodels
        var incomingList = incomingRequests.Items.Select(r => MapToCardViewModel(r, userId)).ToList();
        var outgoingList = outgoingRequests.Items.Select(r => MapToCardViewModel(r, userId)).ToList();

        // Optimization: fetch all conversations for this user once to avoid N+1.
        try
        {
            var convsResult = await _conversationService.GetMyConversationsAsync(userId);
            if (convsResult != null && convsResult.Success && convsResult.Data != null)
            {
                var convsByRequest = convsResult.Data.Where(c => c.RequestId != Guid.Empty).ToDictionary(c => c.RequestId, c => c);

                // Assign conversation ids to approved requests
                foreach (var card in incomingList.Concat(outgoingList))
                {
                    if (card.Status == Ketabi.Core.Domain.Enums.RequestStatus.Approved)
                    {
                        // If conversation exists for the request id, set it
                        if (convsByRequest.TryGetValue(card.RequestId, out var conv))
                        {
                            card.ConversationId = conv.ConversationId;
                        }
                        else
                        {
                            // If not found, check participant-based retrieval as fallback
                            // Do not create conversations here.
                            // Optionally check if user is participant in some conversation related to this request.
                        }
                    }
                }
            }
        }
        catch
        {
            // Non-fatal: leave conversation ids blank if retrieval fails
        }

        var viewModel = new RequestsIndexViewModel
        {
            ActiveTab = tab,
            IncomingRequests = incomingList,
            OutgoingRequests = outgoingList,
            PendingBooks = userPendingBooks.Select(b => MapPendingBookCardViewModel(b)).ToList()
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

            // If request was approved, open (or ensure) a conversation for the request.
            if (parsedStatus == Ketabi.Core.Domain.Enums.RequestStatus.Approved)
            {
                try
                {
                    // OpenConversationAsync is idempotent; if it fails we log and continue.
                    var convResult = await _conversationService.OpenConversationAsync(requestId, userId);
                    if (convResult != null && !convResult.Success)
                    {
                        // Log but do not fail the workflow (status update already succeeded)
                        // Obtain logger via HttpContext.RequestServices if needed
                        var logger = HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<RequestsController>>();
                        logger?.LogWarning("OpenConversationAsync returned failure for request {RequestId}: {Errors}", requestId, string.Join(';', convResult.Errors ?? new List<string>()));
                    }
                }
                catch (Exception ex)
                {
                    var logger = HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<RequestsController>>();
                    logger?.LogError(ex, "Failed to open conversation after approving request {RequestId}", requestId);
                }
            }

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

    private RequestCardViewModel MapToCardViewModel(Ketabi.Application.DTOs.Requests.RequestDto dto, Guid currentUserId, Guid? conversationId = null)
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
            IsIncoming = isIncoming,
            CanAcceptOrReject = isIncoming && dto.Status == RequestStatus.Pending,
            CanWithdraw = !isIncoming && dto.Status == RequestStatus.Pending
        };

        // Assign conversation id if supplied by caller (batch lookup)
        viewModel.ConversationId = conversationId;

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
                IsAvailable = true,
                OwnerId = dto.RequesterId,
                OwnerName = dto.RequesterFullName,
                OwnerAvatarUrl = dto.RequesterAvatarUrl
            };
        }

        return viewModel;
    }

    private PendingBookCardViewModel MapPendingBookCardViewModel(Ketabi.Application.DTOs.Books.PendingBookDto dto)
    {
        return new PendingBookCardViewModel
        {
            BookId = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            Category = dto.Category,
            Condition = dto.Condition,
            TransactionType = dto.TransactionType,
            ImageUrl = dto.ImageUrl ?? string.Empty,
            CoverColor = dto.CoverColor,
            SubmittedAt = DateTime.TryParse(dto.SubmittedAt, out var date) ? date : DateTime.Now,
            Description = dto.Description,
            IsAvailable = dto.IsAvailable
        };
    }
}