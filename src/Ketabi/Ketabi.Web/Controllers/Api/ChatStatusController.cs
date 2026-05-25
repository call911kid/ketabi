using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ketabi.Web.Controllers.Api;

[Route("api/chat/status")]
[ApiController]
public class ChatStatusController : ControllerBase
{
    private readonly IRequestService _requestService;
    private readonly ILogger<ChatStatusController> _logger;

    public ChatStatusController(IRequestService requestService, ILogger<ChatStatusController> logger)
    {
        _requestService = requestService;
        _logger = logger;
    }

    // POST api/chat/status/change
    [HttpPost("change")] 
    public async Task<IActionResult> ChangeStatus([FromForm] string requestId, [FromForm] string newStatus)
    {
        if (!Guid.TryParse(requestId, out var reqGuid)) return BadRequest("Invalid request id");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid)) return Unauthorized();

        try
        {
            // Map friendly names -> RequestStatus
            if (!Enum.TryParse<Ketabi.Core.Domain.Enums.RequestStatus>(newStatus, true, out var parsed))
                return BadRequest("Invalid status");

            await _requestService.UpdateRequestStatusAsync(userGuid, reqGuid, new Ketabi.Application.DTOs.Requests.UpdateRequestStatusDto { Status = parsed });

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change request status {RequestId}", requestId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
