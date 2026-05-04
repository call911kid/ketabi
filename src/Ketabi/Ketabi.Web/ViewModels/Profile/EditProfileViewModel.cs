using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ketabi.Web.ViewModels.Profile;

public class EditProfileViewModel
{
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [MinLength(2,  ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Full name cannot exceed 80 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [MinLength(3,  ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Bio cannot exceed 200 characters.")]
    [Display(Name = "About me")]
    public string? Bio { get; set; }

    [MaxLength(60)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [MaxLength(60)]
    [Display(Name = "Governorate")]
    public string? Governorate { get; set; }

    [Display(Name = "Profile Picture (Optional)")]
    public IFormFile? ProfilePicture { get; set; }
}
