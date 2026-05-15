using Ketabi.Application.DTOs.Auth;

namespace Ketabi.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task RemoveFromRoleAsync(string email, string roleName);
    Task AddToRoleAsync(string email, string roleName);
}
