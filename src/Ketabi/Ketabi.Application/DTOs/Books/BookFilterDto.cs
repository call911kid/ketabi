using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Books;

/// <summary>
/// Filter and paging options for querying book listings.
/// Use this DTO to encapsulate criteria instead of multiple method parameters.
/// </summary>
public class BookFilterDto
{
    /// <summary>Full-text query matching title, author, isbn, or description.</summary>
    public string? Query { get; init; }

    /// <summary>Category id to filter by.</summary>
    public Guid? CategoryId { get; init; }

    /// <summary>Tags to match (any). Empty means no tag filtering.</summary>
    public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Optional listing condition filter.</summary>
    public ListingCondition? Condition { get; init; }

    /// <summary>Optional sharing mode filter (Borrow / Exchange / etc.).</summary>
    public SharingMode? SharingMode { get; init; }

    /// <summary>If provided, only return listings that are currently available.</summary>
    public bool? IsAvailable { get; init; }

    /// <summary>Owner id to filter listings by a specific user.</summary>
    public Guid? OwnerId { get; init; }

    /// <summary>Maximum distance in kilometers from the viewer (requires latitude/longitude to be applied by service).</summary>
    public double? MaxDistanceInKm { get; init; }

    /// <summary>Sort field name (e.g. "listedAt", "distance", "relevance").</summary>
    public string? SortBy { get; init; }

    /// <summary>Sort direction. True = descending.</summary>
    public bool SortDescending { get; init; }

    /// <summary>Pagination: page number (1-based).</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>Pagination: page size.</summary>
    public int PageSize { get; init; } = 20;
}
