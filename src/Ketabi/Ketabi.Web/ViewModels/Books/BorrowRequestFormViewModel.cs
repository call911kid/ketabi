using System.ComponentModel.DataAnnotations;

namespace Ketabi.Web.ViewModels.Books;

public class BorrowRequestFormViewModel
{
    public Guid BookId { get; set; }

    [Required(ErrorMessage = "Please select a return date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Return Date")]
    public DateTime ReturnDate { get; set; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    [Display(Name = "Message to owner (optional)")]
    public string? Note { get; set; }
}
