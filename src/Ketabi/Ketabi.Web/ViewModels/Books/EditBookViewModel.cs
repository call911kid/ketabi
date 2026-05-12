using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ketabi.Web.ViewModels.Books;

public class EditBookViewModel
{
    public Guid BookId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    [MaxLength(200)]
    [Display(Name = "Author")]
    public string Author { get; set; } = string.Empty;

    [MaxLength(13)]
    [Display(Name = "ISBN")]
    public string? ISBN { get; set; }

    [MaxLength(2000)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [MaxLength(60)]
    [Display(Name = "Language")]
    public string? Language { get; set; }

    [MaxLength(100)]
    [Display(Name = "Publisher")]
    public string? Publisher { get; set; }

    [Required]
    [Display(Name = "Category")]
    public Guid CategoryId { get; set; }

    [Required]
    [Display(Name = "Condition")]
    public ListingCondition Condition { get; set; }

    [Required]
    [Display(Name = "Available for")]
    public SharingMode SharingMode { get; set; }

    // For editing, a new cover image is optional since one might already exist.
    [Display(Name = "Cover Image (leave blank to keep current)")]
    public IFormFile? CoverImage { get; set; }

    [Url]
    public string ExistingImageUrl { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "Sharing duration must be between 1 and 365 days.")]
    [Display(Name = "Sharing duration (days)")]
    public int? SharingDurationInDays { get; set; }

    [MaxLength(120)]
    [Display(Name = "Pickup area")]
    public string? LocationNote { get; set; }

    [Display(Name = "Mark as available")]
    public bool IsAvailable { get; set; } = true;

    // Reference Data
    public IList<SelectListItem> CategoryOptions { get; set; } = [];
}
