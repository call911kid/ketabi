namespace Ketabi.Application.DTOs.Reference;

/// <summary>
/// Returned by the category service for filter pill bars and dropdowns.
/// Maps from: Category entity → CategoryFilterItemViewModel / SelectListItem.
/// </summary>
public class CategoryDto
{
    public Guid CategoryId { get; init; }

    /// <summary>Display label, e.g. "Science Fiction".</summary>
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
    public string? IconUrl { get; init; }

    /// <summary>Number of currently available books in this category.</summary>
    public int BookCount { get; init; }
}
