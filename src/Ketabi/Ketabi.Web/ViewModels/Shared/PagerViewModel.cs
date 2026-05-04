namespace Ketabi.Web.ViewModels.Shared;

// View-layer pagination state for rendering pager controls in any list view.
// Populated from PagedResultDto<T>.
public class PagerViewModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrev => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public int PrevPage => CurrentPage - 1;
    public int NextPage => CurrentPage + 1;
}
