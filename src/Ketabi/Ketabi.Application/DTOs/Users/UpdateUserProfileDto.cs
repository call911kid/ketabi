using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// PATCH /profile/edit — partial update of the authenticated user's profile.
/// Maps from: EditProfileViewModel → UpdateUserProfileDto → User entity.
/// All fields are optional; null fields are not updated.
/// </summary>
public class UpdateUserProfileDto
{
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Full name cannot exceed 80 characters.")]
    public string? FullName { get; init; }

    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
    public string? UserName { get; init; }

    [MaxLength(200, ErrorMessage = "Bio cannot exceed 200 characters.")]
    public string? Bio { get; init; }

    [MaxLength(60)]
    public string? City { get; init; }

    [MaxLength(60)]
    public string? Governorate { get; init; }

    /// <summary>
    /// Resolved URL of the uploaded profile picture.
    /// The Web layer resolves IFormFile → URL before populating this field.
    /// </summary>
    [Url]
    [MaxLength(500)]
    public string? ProfilePictureUrl { get; init; }
}
