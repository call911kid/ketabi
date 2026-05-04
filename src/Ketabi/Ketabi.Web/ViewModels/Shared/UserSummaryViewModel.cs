namespace Ketabi.Web.ViewModels.Shared;

public class UserSummaryViewModel
{
    public Guid   UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double ReputationScore { get; set; }
    public int ReviewCount { get; set; }

    // Star rating formatted to 1 decimal place, e.g. "4.8"
    public string FormattedRating => ReputationScore.ToString("F1");

    // Initials fallback when AvatarUrl is empty
    public string Initials => FullName.Length >= 2
        ? $"{FullName[0]}{FullName.Split(' ').LastOrDefault()?[0]}"
        : FullName.Length == 1 ? FullName[0].ToString() : "?";
}
