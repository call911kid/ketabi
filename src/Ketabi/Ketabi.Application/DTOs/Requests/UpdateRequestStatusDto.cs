using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// PATCH /requests/{id}/status — called by the book owner to approve or reject.
/// Maps from: RequestStatusActionViewModel → UpdateRequestStatusDto → Request entity.
/// </summary>
public class UpdateRequestStatusDto
{
    [Required]
    [EnumDataType(typeof(RequestStatus))]
    public RequestStatus Status { get; init; }

    [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters.")]
    public string? Note { get; init; }
}
