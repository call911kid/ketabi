using System.ComponentModel.DataAnnotations;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Auth;

// ── Login ──────────────────────────────────────────────────────────────────

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

// ── Register ───────────────────────────────────────────────────────────────

/// <summary>New account payload. Maps from RegisterViewModel.</summary>
public class RegisterDto
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
    public string UserName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Full name cannot exceed 80 characters.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    public string Password { get; init; } = string.Empty;

    [MaxLength(60)]
    public string? City { get; init; }

    [MaxLength(60)]
    public string? Governorate { get; init; }

    /// <summary>
    /// Publicly accessible URL of the uploaded profile picture.
    /// The Web layer resolves IFormFile → URL before populating this field.
    /// </summary>
    [Url]
    [MaxLength(500)]
    public string? ProfilePictureUrl { get; init; }
}

// ── Auth Result ────────────────────────────────────────────────────────────

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
