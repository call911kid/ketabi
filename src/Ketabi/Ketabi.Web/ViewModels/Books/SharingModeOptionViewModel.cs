namespace Ketabi.Web.ViewModels.Books;

public class SharingModeOptionViewModel
{
    public Ketabi.Core.Domain.Enums.SharingMode Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconHtml { get; set; } = string.Empty;
}
