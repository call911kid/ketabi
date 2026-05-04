using System.ComponentModel.DataAnnotations;

namespace Ketabi.Web.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Full name cannot exceed 80 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(254)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(60)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [MaxLength(60)]
    [Display(Name = "Governorate")]
    public string? Governorate { get; set; }

    // Uses IFormFile? for actual multipart/form-data binding
    [Display(Name = "Profile Picture (Optional)")]
    public IFormFile? ProfilePicture { get; set; }
}
