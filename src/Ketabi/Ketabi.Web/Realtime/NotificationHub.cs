using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ketabi.Web.Realtime;

// [Authorize] enforces authentication at the WebSocket handshake level.
// JwtBearerEvents.OnMessageReceived already reads the ketabi_auth cookie,
// so the JWT middleware populates Context.User before this attribute is checked.
[Authorize]
public sealed class NotificationHub : Hub
{
}
