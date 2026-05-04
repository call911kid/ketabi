using System.ComponentModel.DataAnnotations;
using Ketabi.Application.DTOs.Common;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Queries;

// ── Book Query / Filter ────────────────────────────────────────────────────

/// <summary>
/// Query parameters for the book list (Explorer feed).
/// Maps from: BookFilterViewModel GET query string binding.
/// </summary>
public class BookQueryDto : PagedRequestDto
{
    /// <summary>Free-text search across title, author, and location.</summary>
    [MaxLength(200)]
    public string? SearchQuery { get; init; }

    [EnumDataType(typeof(SharingMode))]
    public SharingMode? SharingMode { get; init; }

    public Guid? CategoryId { get; init; }

    [EnumDataType(typeof(ListingCondition))]
    public ListingCondition? Condition { get; init; }

    /// <summary>Filter by city name.</summary>
    [MaxLength(60)]
    public string? City { get; init; }

    /// <summary>Exclude books owned by the authenticated user. Default: true.</summary>
    public bool ExcludeOwn { get; init; } = true;
}

// ── Request Query / Filter ─────────────────────────────────────────────────

/// <summary>
/// Query parameters for the Requests page list.
/// Maps from: ?tab= query parameter on Requests/Index.
/// </summary>
public class RequestQueryDto : PagedRequestDto
{
    /// <summary>Filter by direction relative to the authenticated user.</summary>
    public RequestDirection? Direction { get; init; }

    [EnumDataType(typeof(RequestStatus))]
    public RequestStatus? Status { get; init; }

    /// <summary>True = borrow requests only. False = exchange requests only. Null = both.</summary>
    public bool? IsBorrow { get; init; }
}

// ── Notification Query ─────────────────────────────────────────────────────

/// <summary>Query parameters for the notifications list.</summary>
public class NotificationQueryDto : PagedRequestDto
{
    /// <summary>When true, returns only unread notifications.</summary>
    public bool UnreadOnly { get; init; }
}

// ── Request Direction ──────────────────────────────────────────────────────

/// <summary>Direction of a request relative to the authenticated user.</summary>
public enum RequestDirection
{
    /// <summary>Requests where the authenticated user is the requester (Outgoing tab).</summary>
    Outgoing = 0,

    /// <summary>Requests where the authenticated user is the book owner (Incoming tab).</summary>
    Incoming = 1
}
