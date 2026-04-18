namespace Polls.Dtos.Auth;

/// <summary>
/// DTO for user registration with only necessary fields.
/// </summary>
public class RegisterUserDto
{
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
