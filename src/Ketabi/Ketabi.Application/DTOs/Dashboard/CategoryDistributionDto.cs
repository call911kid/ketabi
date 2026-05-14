namespace Ketabi.Application.DTOs.Dashboard;

/// <summary>
/// Represents category distribution data for dashboard visualization.
/// </summary>
public class CategoryDistributionDto
{
    public string Name { get; init; } = string.Empty;
    public int Value { get; init; }
    
}