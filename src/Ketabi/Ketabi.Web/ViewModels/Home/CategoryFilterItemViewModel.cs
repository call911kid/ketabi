namespace Ketabi.Web.ViewModels.Home;

public class CategoryFilterItemViewModel
{
    public Guid? CategoryId { get; set; }   // null = "All" sentinel
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int BookCount { get; set; }

    // True when this pill is the currently selected filter
    public bool IsActive { get; set; }
}
