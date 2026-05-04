namespace Ketabi.Application.DTOs.Reference;

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
