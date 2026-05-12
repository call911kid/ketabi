using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// PATCH /profile/edit — partial update of the authenticated user's profile.
/// Maps from: EditProfileViewModel → UpdateUserProfileDto → User entity.
/// All fields are optional; null fields are not updated.
/// </summary>
public class UpdateUserProfileDto
{

    [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "First name cannot exceed 80 characters.")]
    public string? FirstName { get; init; }
    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Last name cannot exceed 80 characters.")]
    public string? LastName { get; init; }

    //[EmailAddress(ErrorMessage = "Invalid email address.")]
    //public string? Email { get; init; }

    [MaxLength(200, ErrorMessage = "Bio cannot exceed 200 characters.")]
    public string? Bio { get; set; }

    [MaxLength(60)]
    public string? City { get; set; }

    [MaxLength(60)]
    public string? Governorate { get; set; }

    /// <summary>
    /// Resolved URL of the uploaded profile picture.
    /// The Web layer resolves IFormFile → URL before populating this field.
    /// </summary>
    [Url]
    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; }
}
