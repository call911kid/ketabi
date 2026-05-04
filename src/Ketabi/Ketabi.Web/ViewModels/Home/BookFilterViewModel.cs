using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Web.ViewModels.Home;

public class BookFilterViewModel
{
    [MaxLength(200)]
    public string? SearchQuery { get; set; }

    public SharingMode? SharingMode { get; set; }
    public Guid? CategoryId { get; set; }
    public ListingCondition? Condition { get; set; }
    public string? City { get; set; }

    // Pagination controls
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
