using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Auth;

/// <summary>
/// Returned on successful authentication.
/// Contains enough user info to bootstrap the navbar and redirect logic.
/// </summary>
public class AuthResultDto
{
    /// <summary>Lightweight user snapshot for populating NavbarViewModel.</summary>
    public UserSummaryDto User { get; init; } = new();

    /// <summary>True on first-time registration (used for redirect logic).</summary>
    public bool IsNewUser { get; init; }
}
