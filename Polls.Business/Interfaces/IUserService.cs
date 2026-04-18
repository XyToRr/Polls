using Polls.Core.Models.Users;

namespace Polls.Business.Interfaces;

/// <summary>
/// Interface for user management service.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by login.
    /// </summary>
    /// <param name="login">User login</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetUserByLoginAsync(string login);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">User to create</param>
    Task<User> CreateAsync(User user);
}
