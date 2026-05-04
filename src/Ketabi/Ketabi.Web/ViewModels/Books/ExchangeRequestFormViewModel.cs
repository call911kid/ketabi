using System.ComponentModel.DataAnnotations;

namespace Ketabi.Web.ViewModels.Books;

public class ExchangeRequestFormViewModel
{
    public Guid BookId { get; set; }

    [Required(ErrorMessage = "Please select a book to offer.")]
    [Display(Name = "Book to offer")]
    public Guid OfferedBookId { get; set; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    [Display(Name = "Message to owner (optional)")]
    public string? Note { get; set; }
}
