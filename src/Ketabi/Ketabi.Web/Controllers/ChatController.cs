using Ketabi.Application.DTOs.Chat;
using Ketabi.Application.DTOs.Reviews;
using Ketabi.Application.Interfaces;
using Ketabi.Web.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ketabi.Web.Controllers;

[Authorize]
public class ChatController : BaseController
{
    private readonly IConversationService _conversationService;
    private readonly IReviewService _reviewService;

    public ChatController(IConversationService conversationService, IReviewService reviewService)
    {
        _conversationService = conversationService;
        _reviewService = reviewService;
    }

    // GET /Chat
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return RedirectToAction("Login", "Account");

        // BUG 8: expose current user id for the view / data attribute
        ViewData["CurrentUserId"] = userId.ToString();

        var result = await _conversationService.GetMyConversationsAsync(userId);
        if (!result.Success) return View(new ChatIndexViewModel());

        var vm = new ChatIndexViewModel
        {
            Conversations = result.Data!.Select(c => MapToSummary(c, userId)).ToList(),
            ActiveConversation = null
        };

        return View(vm);
    }

    // GET /Chat/{id}
    [HttpGet]
    [Route("Chat/{id}")]
    public async Task<IActionResult> Index(string id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return RedirectToAction("Login", "Account");

        // BUG 8: expose current user id for the view / data attribute
        ViewData["CurrentUserId"] = userId.ToString();

        if (!Guid.TryParse(id, out var convGuid))
            return RedirectToAction(nameof(Index));

        var listResult   = await _conversationService.GetMyConversationsAsync(userId);
        var detailResult = await _conversationService.GetConversationAsync(convGuid, userId);

        if (!detailResult.Success) return RedirectToAction(nameof(Index));

        var conv = detailResult.Data!;

        // Check if the current user has already reviewed this request
        bool reviewAlreadySubmitted = false;
        if (Guid.TryParse(conv.RequestId.ToString(), out var reqGuid))
            reviewAlreadySubmitted = await _reviewService.HasReviewedAsync(userId, reqGuid);

        var vm = new ChatIndexViewModel
        {
            Conversations = listResult.Success
                ? listResult.Data!.Select(c => MapToSummary(c, userId)).ToList()
                : new List<ConversationSummaryViewModel>(),
            ActiveConversation = MapToDetail(conv, userId, reviewAlreadySubmitted)
        };

        // mark selected
        var selected = vm.Conversations.FirstOrDefault(c => c.ConversationId == id);
        if (selected != null) selected.IsSelected = true;

        return View(vm);
    }

    // POST /Chat/Open?requestId={id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(Guid requestId)
    {
        var userId = GetCurrentUserId();
        var result = await _conversationService.OpenConversationAsync(requestId, userId);

        if (!result.Success)
        {
            TempData["FlashError"] = result.Errors.FirstOrDefault();
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index),
            new { id = result.Data!.ConversationId });
    }

    // POST /Chat/ConfirmHandoff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmHandoff(string conversationId)
    {
        var userId = GetCurrentUserId();

        if (!Guid.TryParse(conversationId, out var convGuid))
            return BadRequest();

        var result = await _conversationService.ConfirmHandoffAsync(
            new Application.DTOs.Chat.ConfirmHandoffDto { ConversationId = convGuid }, userId);

        if (!result.Success)
            TempData["FlashError"] = result.Errors.FirstOrDefault();
        else
            TempData["FlashSuccess"] = "Handoff confirmed!";

        return RedirectToAction(nameof(Index), new { id = conversationId });
    }

    // POST /Chat/SubmitReview
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(
        string conversationId,
        string revieweeId,
        string requestId,
        int rating,
        string? comment)
    {
        var callerId = GetCurrentUserId();

        if (!Guid.TryParse(revieweeId, out var revieweeGuid) ||
            !Guid.TryParse(requestId,  out var requestGuid))
        {
            TempData["FlashError"] = "Invalid review data.";
            return RedirectToAction(nameof(Index), new { id = conversationId });
        }

        var dto = new CreateReviewDto
        {
            RevieweeId       = revieweeGuid,
            RelatedRequestId = requestGuid,
            Rating           = rating,
            Comment          = comment
        };

        var result = await _reviewService.CreateReviewAsync(callerId, dto);

        if (!result.Success)
            TempData["FlashError"] = result.Errors.FirstOrDefault() ?? "Review could not be submitted.";
        else
            TempData["FlashSuccess"] = "Review submitted! Thank you for your feedback.";

        return RedirectToAction(nameof(Index), new { id = conversationId });
    }

    // Mapping helpers
    private static ConversationSummaryViewModel MapToSummary(ConversationDto c, Guid callerId)
    {
        var isOwner = callerId == c.OwnerId;
        var otherUser = new OtherUserViewModel
        {
            UserId = (isOwner ? c.RequesterId : c.OwnerId).ToString(),
            FullName = isOwner ? c.RequesterName : c.OwnerName,
            AvatarUrl = isOwner ? c.RequesterAvatar : c.OwnerAvatar
        };

        return new ConversationSummaryViewModel
        {
            ConversationId = c.ConversationId.ToString(),
            Book = new BookSummaryViewModel { Title = c.BookTitle, CoverImageUrl = c.BookImageUrl ?? string.Empty },
            OtherUser = otherUser,
            TransactionStatus = MapStatus(c),
            LastMessagePreview = c.LastMessage?.Text ?? string.Empty,
            LastMessageIsMine = c.LastMessage != null ? (c.LastMessage.SenderId == callerId) : false,
            LastMessageTimeAgo = c.LastMessage?.TimeAgo ?? string.Empty,
            UnreadCount = c.UnreadCount
        };
    }

    private static ConversationDetailViewModel MapToDetail(ConversationDto c, Guid callerId, bool reviewAlreadySubmitted = false)
    {
        var isOwner = callerId == c.OwnerId;

        var otherUser = new OtherUserViewModel
        {
            UserId = (isOwner ? c.RequesterId : c.OwnerId).ToString(),
            FullName = isOwner ? c.RequesterName : c.OwnerName,
            AvatarUrl = isOwner ? c.RequesterAvatar : c.OwnerAvatar,
        };

        // BUG 5 + BUG 7: Build ordered list once to compare adjacent messages
        var ordered = c.Messages.OrderBy(m => m.CreatedAt).ToList();

        var messages = ordered.Select((m, index) =>
        {
            var prev = index > 0 ? ordered[index - 1] : null;

            // BUG 5: ShowDateDivider is true when this message starts a new calendar day
            var showDivider = prev == null || m.CreatedAt.Date != prev.CreatedAt.Date;

            return new MessageViewModel
            {
                MessageId       = m.MessageId.ToString(),
                SenderId        = m.SenderId.ToString(),
                IsMine          = m.SenderId == callerId,
                SenderName      = m.SenderName,
                SenderAvatarUrl = m.SenderAvatar,
                Text            = m.Text,
                FormattedTime   = m.CreatedAt.ToString("hh:mm tt"),
                DateLabel       = m.CreatedAt.Date == DateTime.UtcNow.Date
                                      ? "Today"
                                      : m.CreatedAt.Date == DateTime.UtcNow.Date.AddDays(-1)
                                          ? "Yesterday"
                                          : m.CreatedAt.ToString("MMM d"),
                ShowDateDivider = showDivider,
                ShowTimestamp   = true   // placeholder; corrected in the post-processing loop below
            };
        }).ToList();

        // BUG 7: ShowTimestamp — only show at the last bubble of a consecutive sender run,
        // or when the next message switches sender, or at the very last message.
        for (var i = 0; i < messages.Count; i++)
        {
            var isLast              = i == messages.Count - 1;
            var nextIsDifferentSender = !isLast && messages[i + 1].IsMine != messages[i].IsMine;
            messages[i].ShowTimestamp = isLast || nextIsDifferentSender;
        }

        var vm = new ConversationDetailViewModel
        {
            ConversationId = c.ConversationId.ToString(),
            Book           = new BookSummaryViewModel { Title = c.BookTitle, CoverImageUrl = c.BookImageUrl ?? string.Empty },
            OtherUser      = otherUser,
            TransactionStatus          = MapStatus(c),
            RequestId = c.RequestId.ToString(),
            CurrentUserConfirmedHandoff = isOwner ? c.OwnerConfirmedHandoff  : c.RequesterConfirmedHandoff,
            OtherUserConfirmedHandoff  = isOwner ? c.RequesterConfirmedHandoff : c.OwnerConfirmedHandoff,
            CurrentUserAvatarUrl = isOwner ? c.OwnerAvatar : c.RequesterAvatar,
            ReviewAlreadySubmitted = reviewAlreadySubmitted,
            RequestType = Enum.TryParse<RequestType>(c.RequestType, true, out var rType) ? rType : RequestType.Exchange,
            BorrowDurationDays = c.BorrowDurationDays,
            Messages = messages
        };

        return vm;
    }

    private static TransactionStatus MapStatus(ConversationDto c)
    {
        if (c.RequesterConfirmedHandoff && c.OwnerConfirmedHandoff)
            return TransactionStatus.Completed;
        if (c.RequesterConfirmedHandoff)
            return TransactionStatus.HandoffConfirmedRequester;
        if (c.OwnerConfirmedHandoff)
            return TransactionStatus.HandoffConfirmedOwner;
        return TransactionStatus.Active;
    }

    private Guid GetCurrentUserId()
    {
        var s = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(s, out var id) ? id : Guid.Empty;
    }
}