namespace Ketabi.Application.DTOs.Users;

// Use CreatedUserDto to express a created user response. It reuses UserDto fields and
// adds server-generated values when necessary.
public sealed class CreatedUserDto : UserDto
{
    // include server-side properties if any (e.g., default reputation)
    public double ReputationScore { get; init; }
}
