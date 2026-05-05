using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// Full request record returned by GET /requests and GET /requests/{id}.
/// </summary>
public class RequestDto
{
    public Guid RequestId { get; init; }
    public RequestType Type { get; init; }
    public RequestStatus Status { get; init; }
    public DateTime RequestDate { get; init; }
    public string? Note { get; init; }
    public DateTime? ReturnDate { get; init; }

    public Guid ListingId { get; init; }
    public string ListingTitle { get; init; } = string.Empty;
    public string ListingAuthor { get; init; } = string.Empty;
    public string? ListingImageUrl { get; init; }
    public string ListingCategory { get; init; } = string.Empty;
    public ListingCondition ListingCondition { get; init; }
    public SharingMode ListingSharingMode { get; init; }

    public Guid RequesterId { get; init; }
    public string RequesterFullName { get; init; } = string.Empty;
    public string RequesterUserName { get; init; } = string.Empty;
    public string RequesterAvatarUrl { get; init; } = string.Empty;
    public string RequesterLocation { get; init; } = string.Empty;
    public double RequesterReputationScore { get; init; }
    public int RequesterReviewCount { get; init; }
    public int RequesterTradesCount { get; init; }

    public Guid OwnerId { get; init; }
    public string OwnerFullName { get; init; } = string.Empty;
    public string OwnerUserName { get; init; } = string.Empty;
    public string OwnerAvatarUrl { get; init; } = string.Empty;
    public string OwnerLocation { get; init; } = string.Empty;
    public double OwnerReputationScore { get; init; }
    public int OwnerReviewCount { get; init; }
    public int OwnerTradesCount { get; init; }

    public Guid? OfferedListingId { get; init; }
    public string? OfferedListingTitle { get; init; }
    public string? OfferedListingAuthor { get; init; }
    public string? OfferedListingImageUrl { get; init; }
    public string? OfferedListingCategory { get; init; }
    public ListingCondition? OfferedListingCondition { get; init; }
    public SharingMode? OfferedListingSharingMode { get; init; }
}
