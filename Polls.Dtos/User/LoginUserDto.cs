namespace Polls.Dtos.User;

/// <summary>
/// DTO for user login with login and password only.
/// </summary>
public class LoginUserDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
