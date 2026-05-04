using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ketabi.Web.ViewModels.Books;

public class CreateBookViewModel
{
    // Step 1: Book Information

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1)]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    [MaxLength(200, ErrorMessage = "Author cannot exceed 200 characters.")]
    [Display(Name = "Author")]
    public string Author { get; set; } = string.Empty;

    [MaxLength(13, ErrorMessage = "ISBN cannot exceed 13 characters.")]
    [Display(Name = "ISBN (optional)")]
    public string? ISBN { get; set; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [MaxLength(60)]
    [Display(Name = "Language")]
    public string? Language { get; set; }

    [MaxLength(100)]
    [Display(Name = "Publisher")]
    public string? Publisher { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public Guid CategoryId { get; set; }

    // Step 2: Listing Settings

    [Required(ErrorMessage = "Condition is required.")]
    [Display(Name = "Condition")]
    public ListingCondition Condition { get; set; }

    [Required(ErrorMessage = "Sharing mode is required.")]
    [Display(Name = "Available for")]
    public SharingMode SharingMode { get; set; }

    // Step 3: Cover Image & Location

    // Uses IFormFile for multipart/form-data binding
    [Required(ErrorMessage = "Book cover image is required.")]
    [Display(Name = "Cover Image")]
    public IFormFile CoverImage { get; set; } = null!;

    [MaxLength(120)]
    [Display(Name = "Pickup area")]
    public string? LocationNote { get; set; }

    // Reference Data (populated by controller, not submitted)

    // Category dropdown/pill options.
    public IList<SelectListItem>       CategoryOptions       { get; set; } = [];

    // Condition selection cards with label, description, and badge CSS.
    public IList<ConditionOptionViewModel> ConditionOptions  { get; set; } = [];

    // SharingMode selection cards.
    public IList<SharingModeOptionViewModel> SharingOptions  { get; set; } = [];
}
