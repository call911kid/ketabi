using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ketabi.Web.ViewModels.Books;

public class BookDetailViewModel
{
    // Core Book Data
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public string? Publisher { get; set; }
    public string Category { get; set; } = string.Empty;
    public ListingCondition Condition { get; set; }
    public SharingMode SharingMode { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? LocationNote { get; set; }

    // Owner
    public UserSummaryViewModel Owner { get; set; } = new();

    // Request Action Panel State

    // True when the viewing user IS the book owner.
    public bool IsOwner { get; set; }

    // True when the viewer has a pending/accepted request on this book.
    public bool HasActiveRequest { get; set; }

    // Populated when HasActiveRequest = true.
    public RequestStatus? ViewerRequestStatus { get; set; }

    // True when SharingMode allows borrow.
    public bool CanBorrow => SharingMode is SharingMode.Borrow or SharingMode.Both;

    // True when SharingMode allows exchange.
    public bool CanExchange => SharingMode is SharingMode.Exchange or SharingMode.Both;

    // Borrow Form Data

    // Duration options for the borrow form dropdown.
    public IList<SelectListItem> BorrowDurationOptions { get; set; } = [];
    public BorrowRequestFormViewModel BorrowRequest { get; set; } = new();

    // Exchange Form Data

    // Books owned by the viewer that are available to offer in exchange.
    public IList<BookCardViewModel> ViewerAvailableBooks { get; set; } = [];
    public ExchangeRequestFormViewModel ExchangeRequest { get; set; } = new();

    // Related Books
    public IList<BookCardViewModel> RelatedBooks { get; set; } = [];

    // Display Helpers
    public string ConditionBadgeCss => Condition switch
    {
        ListingCondition.New  => "badge-new",
        ListingCondition.Good => "badge-good",
        ListingCondition.Fair => "badge-fair",
        ListingCondition.Poor => "badge-worn",
        _                     => string.Empty
    };

    public string SharingModeBadgeCss => SharingMode switch
    {
        SharingMode.Borrow   => "badge-borrow",
        SharingMode.Exchange => "badge-exchange",
        SharingMode.Both     => "badge-both",
        _                    => string.Empty
    };
}
