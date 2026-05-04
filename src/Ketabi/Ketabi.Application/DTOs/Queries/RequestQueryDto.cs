using System.ComponentModel.DataAnnotations;
using Ketabi.Application.DTOs.Common;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Queries;

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
