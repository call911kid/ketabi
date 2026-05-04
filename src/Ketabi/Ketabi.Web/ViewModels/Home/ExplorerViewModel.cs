using Ketabi.Web.ViewModels.Shared;

namespace Ketabi.Web.ViewModels.Home;

public class ExplorerViewModel
{
    // Book Grid
    public IList<BookCardViewModel> Books { get; set; } = [];
    public PagerViewModel Pager { get; set; } = new();

    // Active Filter State
    public BookFilterViewModel Filter { get; set; } = new();

    // Reference Data for Filter Bar
    public IList<CategoryFilterItemViewModel> Categories { get; set; } = [];

    // True when no books match the current filter combination.
    public bool IsEmptyState => !Books.Any();

    // True when any non-default filter is active (drives "Clear Filters" button visibility).
    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(Filter.SearchQuery)
        || Filter.SharingMode.HasValue
        || Filter.CategoryId.HasValue
        || Filter.Condition.HasValue;
}
