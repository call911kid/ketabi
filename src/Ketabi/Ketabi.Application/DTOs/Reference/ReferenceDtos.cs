namespace Ketabi.Application.DTOs.Reference;

// ── Category ───────────────────────────────────────────────────────────────

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

// ── Borrow Duration Option ─────────────────────────────────────────────────

/// <summary>
/// Maps UI-friendly labels to canonical day counts for the borrow duration dropdown.
/// Maps to: SelectListItem in BookDetailViewModel.BorrowDurationOptions.
/// </summary>
public class BorrowDurationOptionDto
{
    /// <summary>Human-readable label, e.g. "2 Weeks".</summary>
    public string Label { get; init; } = string.Empty;

    /// <summary>Number of calendar days, e.g. 14.</summary>
    public int Days { get; init; }
}
