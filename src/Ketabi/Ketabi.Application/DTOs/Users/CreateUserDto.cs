namespace Ketabi.Application.DTOs.Users;

// Base DTO for user identity/profile data used across the application layer.
public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

// Specific DTO used when creating a user. Inherits common shape from UserDto.
public sealed class CreateUserDto : UserDto { }
