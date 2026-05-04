using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Common;

/// <summary>Standard envelope for all service operation results.</summary>
public class ServiceResultDto<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ServiceResultDto<T> Ok(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ServiceResultDto<T> Fail(string error)
        => new() { Success = false, Errors = [error] };

    public static ServiceResultDto<T> Fail(IEnumerable<string> errors)
        => new() { Success = false, Errors = errors.ToList() };
}

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

/// <summary>Paged list response envelope returned by service list methods.</summary>
public class PagedResponseDto<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPrevPage => Page > 1;
}
