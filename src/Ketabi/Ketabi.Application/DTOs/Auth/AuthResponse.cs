namespace Ketabi.Application.DTOs.Auth;

public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Roles { get; set; } = new List<string>();
}
