using Microsoft.AspNetCore.Http;

namespace Ketabi.Application.DTOs.Auth;

public sealed class RegisterRequest
{
    public string UserName { get; set; }
    public string Email { get; set; } 
    public string Password { get; set; } 
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
