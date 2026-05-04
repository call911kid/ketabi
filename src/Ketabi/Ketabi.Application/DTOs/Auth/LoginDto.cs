using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Auth;

/// <summary>Credentials submitted by the login form. Maps from LoginViewModel.</summary>
public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [MaxLength(128)]
    public string Password { get; init; } = string.Empty;

    public bool RememberMe { get; init; }
}
