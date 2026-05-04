using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Books;

/// <summary>
/// Viewer's active request state on a book. Embedded inside BookDetailDto
/// to drive the request action panel in BookDetailViewModel.
/// </summary>
public class ActiveRequestInfoDto
{
    /// <summary>True when the authenticated viewer has an active request on this book.</summary>
    public bool HasActiveRequest { get; init; }
    public RequestStatus? ViewerRequestStatus { get; init; }

    /// <summary>True when the authenticated viewer owns the book.</summary>
    public bool IsOwner { get; init; }
}
