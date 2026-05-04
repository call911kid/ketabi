using System.ComponentModel.DataAnnotations;
using Ketabi.Application.DTOs.Common;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Queries;

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
