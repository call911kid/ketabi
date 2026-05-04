namespace Ketabi.Web.ViewModels.Shared;

// Injected into _Layout.cshtml via ViewData or a base controller property.
// Drives the navbar profile dropdown and notification badge.
// Null when the user is a guest (IsAuthenticated = false).
public class NavbarViewModel
{
    public bool IsAuthenticated { get; set; }
    public Guid CurrentUserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public double ReputationScore { get; set; }
    public int ReviewCount { get; set; }
    public int BooksListed { get; set; }
    public int CompletedTransactions { get; set; }
    public int UnreadNotifications { get; set; }

    // True when the current route is the Explorer (Home/Index) — used to suppress the search bar duplicate
    public bool IsExplorerPage { get; set; }
}
