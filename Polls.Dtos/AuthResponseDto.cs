namespace Polls.Dtos.Auth;

/// <summary>
/// DTO for authentication response with JWT token only.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}
