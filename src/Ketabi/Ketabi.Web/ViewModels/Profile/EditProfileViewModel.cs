using System.ComponentModel.DataAnnotations;

namespace Ketabi.Web.ViewModels.Profile;

public class EditProfileViewModel
{
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "First name cannot exceed 80 characters.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Last name cannot exceed 80 characters.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    //[MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    //[MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
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
