namespace Ketabi.Web.ViewModels.Requests;

/// <summary>
/// Represents a book pending admin approval for display on the requests page.
/// </summary>
public class PendingBookCardViewModel
{
    public string BookId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string CoverColor { get; set; } = "#F5DEB3";
    public DateTime SubmittedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
