using Polls.Dtos.Auth;

namespace Polls.Business.Interfaces;

/// <summary>
/// Interface for authentication service.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and returns JWT token.
    /// </summary>
    /// <param name="dto">Registration data</param>
    /// <returns>JWT token for the new user</returns>
    Task<string> RegisterAsync(RegisterUserDto dto);

    /// <summary>
    /// Authenticates user and returns JWT token.
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>JWT token if credentials are valid</returns>
    Task<string> LoginAsync(LoginUserDto dto);
}
