namespace Ketabi.Application.DTOs.Category;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string Color { get; set; } = string.Empty;
}
