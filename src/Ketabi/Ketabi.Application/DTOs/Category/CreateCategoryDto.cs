namespace Ketabi.Application.DTOs.Category;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }  
    public string? IconUrl { get; set; }
}
