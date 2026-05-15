namespace Ketabi.Application.DTOs.Dashboard;

/// <summary>
/// Represents monthly user and book growth statistics for the admin dashboard.
/// </summary>
public class UserGrowthDto
{
    public string Month { get; init; } = string.Empty;
    public int NumberOfUsers { get; init; }
    public int NumberOfBooks { get; init; }
}