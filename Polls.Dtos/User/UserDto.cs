namespace Polls.Dtos.User;

/// <summary>
/// DTO for public user representation (without password).
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Login { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
