using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ketabi.Web.ViewModels.Chat;

namespace Ketabi.Web.Controllers;

/// <summary>
/// Chat page controller — currently returns mock data only.
/// Real data wiring will be added in a future sprint.
/// </summary>
[Authorize]
public class ChatController : BaseController
{
    // GET /Chat  (no conversation selected)
    public IActionResult Index() => View(BuildMockViewModel(null));

    // GET /Chat/{id}  (conversation selected)
    [HttpGet("{id}")]
    [Route("Chat/{id}")]
    public IActionResult Index(string id) => View(BuildMockViewModel(id));

    // POST /Chat/ConfirmHandoff  (stub — wired in future sprint)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmHandoff(string conversationId)
    {
        // TODO: wire up service call
        TempData["FlashSuccess"] = "Handoff confirmed!";
        return RedirectToAction(nameof(Index), new { id = conversationId });
    }

    // POST /Chat/SubmitReview  (stub — wired in future sprint)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitReview(string conversationId, string revieweeId, int rating, string? comment)
    {
        // TODO: wire up service call
        TempData["FlashSuccess"] = "Review submitted!";
        return RedirectToAction(nameof(Index), new { id = conversationId });
    }

    // ── Mock data factory ───────────────────────────────────────────────────
    private static ChatIndexViewModel BuildMockViewModel(string? activeId)
    {
        // Two mock conversations matching the UI screenshot
        var conversations = new List<ConversationSummaryViewModel>
        {
            new()
            {
                ConversationId       = "conv-1",
                Book                 = new() { BookId = "b1", Title = "Being and Time",  CoverImageUrl = "/img/mock/being-and-time.jpg" },
                OtherUser            = new() { UserId = "u1", FullName = "Omar Khalid",  AvatarUrl = "/img/mock/omar.jpg", Location = "Zamalek, Cairo", Rating = 4.8 },
                TransactionStatus    = TransactionStatus.HandoffConfirmedRequester,
                LastMessagePreview   = "nice",
                LastMessageIsMine    = true,
                LastMessageTimeAgo   = "5h ago",
                IsSelected           = activeId == "conv-1",
            },
            new()
            {
                ConversationId       = "conv-2",
                Book                 = new() { BookId = "b2", Title = "The Alchemist",   CoverImageUrl = "/img/mock/alchemist.jpg" },
                OtherUser            = new() { UserId = "u2", FullName = "Youssef Nour", AvatarUrl = "/img/mock/youssef.jpg", Location = "Maadi, Cairo",   Rating = 4.5 },
                TransactionStatus    = TransactionStatus.Active,
                LastMessagePreview   = "Yes, absolutely! When can you pick it up?",
                LastMessageIsMine    = true,
                LastMessageTimeAgo   = "5h ago",
                IsSelected           = activeId == "conv-2",
            },
        };

        ConversationDetailViewModel? active = activeId switch
        {
            "conv-1" => BuildConvDetail_Omar(),
            "conv-2" => BuildConvDetail_Youssef(),
            _        => null
        };

        return new ChatIndexViewModel
        {
            Conversations      = conversations,
            ActiveConversation = active,
        };
    }

    private static ConversationDetailViewModel BuildConvDetail_Omar() => new()
    {
        ConversationId               = "conv-1",
        Book                         = new() { BookId = "b1", Title = "Being and Time",  CoverImageUrl = "/img/mock/being-and-time.jpg" },
        OtherUser                    = new() { UserId = "u1", FullName = "Omar Khalid",  AvatarUrl = "/img/mock/omar.jpg", Location = "Zamalek, Cairo", Rating = 4.8 },
        TransactionStatus            = TransactionStatus.HandoffConfirmedRequester,
        RequestType                  = RequestType.Exchange,
        CurrentUserConfirmedHandoff  = true,
        OtherUserConfirmedHandoff    = false,
        ReviewAlreadySubmitted       = false,
        Messages                     = new List<MessageViewModel>
        {
            new() { MessageId="m1", IsMine=false, SenderName="Omar Khalid",  SenderAvatarUrl="/img/mock/omar.jpg",   Text="Perfect! Saturday at 10am works great. Shall we meet at Cilantro in Zamalek? It's halfway for us.", FormattedTime="01:35 PM", DateLabel="Yesterday", ShowDateDivider=true  },
            new() { MessageId="m2", IsMine=true,  SenderName="Layla Hassan", SenderAvatarUrl="/img/mock/layla.jpg", Text="That sounds wonderful! See you there. I'll have The Alchemist wrapped up nicely 📚",                 FormattedTime="01:40 PM", DateLabel="Yesterday", ShowDateDivider=false },
            new() { MessageId="m3", IsMine=false, SenderName="Omar Khalid",  SenderAvatarUrl="/img/mock/omar.jpg",   Text="Haha love the enthusiasm! Being and Time will be equally well-packaged. Looking forward to it!",    FormattedTime="01:42 PM", DateLabel="Yesterday", ShowDateDivider=false },
            new() { MessageId="m4", IsMine=true,  SenderName="Layla Hassan", SenderAvatarUrl="/img/mock/layla.jpg", Text="nice",                                                                                                FormattedTime="05:10 PM", DateLabel="Today",     ShowDateDivider=true  },
        },
    };

    private static ConversationDetailViewModel BuildConvDetail_Youssef() => new()
    {
        ConversationId     = "conv-2",
        Book               = new() { BookId = "b2", Title = "The Alchemist", CoverImageUrl = "/img/mock/alchemist.jpg" },
        OtherUser          = new() { UserId = "u2", FullName = "Youssef Nour", AvatarUrl = "/img/mock/youssef.jpg", Location = "Maadi, Cairo", Rating = 4.5 },
        TransactionStatus  = TransactionStatus.Active,
        RequestType        = RequestType.Borrow,
        BorrowDurationDays = 14,
        Messages           = new List<MessageViewModel>
        {
            new() { MessageId="m5", IsMine=false, SenderName="Youssef Nour", SenderAvatarUrl="/img/mock/youssef.jpg", Text="Hey! Is the book still available?",         FormattedTime="10:00 AM", DateLabel="Today", ShowDateDivider=true  },
            new() { MessageId="m6", IsMine=true,  SenderName="Layla Hassan", SenderAvatarUrl="/img/mock/layla.jpg",  Text="Yes, absolutely! When can you pick it up?", FormattedTime="10:05 AM", DateLabel="Today", ShowDateDivider=false },
        },
    };
}
