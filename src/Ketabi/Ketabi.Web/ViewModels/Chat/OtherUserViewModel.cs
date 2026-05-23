namespace Ketabi.Web.ViewModels.Chat;

public class OtherUserViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Rating { get; set; }

    /// <summary>Formatted to 1 decimal: "4.8". Empty string when Rating == 0.</summary>
    public string FormattedRating => Rating > 0 ? Rating.ToString("F1") : string.Empty;
}
