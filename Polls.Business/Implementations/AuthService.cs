using Polls.Business.Interfaces;
using Polls.Core.Models.Users;
using Polls.Dtos.User;

namespace Polls.Business.Implementations;

/// <summary>
/// Authentication service with registration and login.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserService userService,
        IPasswordHashService passwordHashService,
        IJwtTokenService jwtTokenService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }

    public async Task<string> RegisterAsync(RegisterUserDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new ArgumentException("Login is required", nameof(dto.Login));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required", nameof(dto.Password));

        var existingUser = await _userService.GetUserByLoginAsync(dto.Login);
        if (existingUser != null)
            throw new InvalidOperationException($"User with login '{dto.Login}' already exists");

        var hashedPassword = _passwordHashService.HashPassword(dto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            LastName = dto.LastName,
            Login = dto.Login,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        await _userService.CreateAsync(user);

        return _jwtTokenService.GenerateToken(user);
    }

    public async Task<string> LoginAsync(LoginUserDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new ArgumentException("Login is required", nameof(dto.Login));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required", nameof(dto.Password));

        var user = await _userService.GetUserByLoginAsync(dto.Login);
        if (user == null)
            throw new InvalidOperationException($"User with login '{dto.Login}' not found");

        var isPasswordValid = _passwordHashService.VerifyPassword(dto.Password, user.Password);
        if (!isPasswordValid)
            throw new InvalidOperationException("Invalid password");

        return _jwtTokenService.GenerateToken(user);
    }
}
