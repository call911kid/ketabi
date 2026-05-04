using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Common;

/// <summary>Inbound pagination + sort parameters for list queries.</summary>
public class PagedRequestDto
{
    /// <summary>1-based page number. Default: 1.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; init; } = 1;

    /// <summary>Records per page. Default: 20. Maximum: 100.</summary>
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; init; } = 20;

    /// <summary>Optional field name to sort by.</summary>
    public string? SortBy { get; init; }

    /// <summary>Sort direction. true = ascending (default).</summary>
    public bool Ascending { get; init; } = true;
}
