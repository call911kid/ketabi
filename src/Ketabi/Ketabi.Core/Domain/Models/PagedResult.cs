namespace Ketabi.Core.Domain.Models;

public record PagedResult<T>
(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
) where T : class;