using Ketabi.Core.Domain.Enums;

namespace Ketabi.Web.ViewModels.Books;

public class ConditionOptionViewModel
{
    public ListingCondition Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BadgeCss { get; set; } = string.Empty;
}
