using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Web.ViewModels.Requests;

// Submitted by the owner Accept / Reject buttons on _RequestCard.cshtml.
// Maps to UpdateRequestStatusDto.
public class RequestStatusActionViewModel
{
    public Guid RequestId { get; set; }
    public RequestStatus Status { get; set; }

    [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters.")]
    public string? Note { get; set; }
}
