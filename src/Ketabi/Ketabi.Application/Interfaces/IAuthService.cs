using Ketabi.Application.DTOs.Auth;
using System.Collections.Generic;

namespace Ketabi.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<IReadOnlyCollection<string>> GetRolesAsync(Guid userId);
    Task RemoveFromRoleAsync(string email, string roleName);
    Task AddToRoleAsync(string email, string roleName);
}
