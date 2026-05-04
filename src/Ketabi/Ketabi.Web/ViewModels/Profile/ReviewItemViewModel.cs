namespace Ketabi.Web.ViewModels.Profile;

public class ReviewItemViewModel
{
    public Guid ReviewId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerAvatar { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string RelatedBookTitle { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;

    // Star count for rendering filled/empty star icons.
    public IEnumerable<bool> Stars => Enumerable.Range(1, 5).Select(i => i <= Rating);
}
