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

    /// <summary>
    /// Adds a follow relationship between two users.
    /// </summary>
    /// <param name="currentUserId">ID of the user who is following</param>
    /// <param name="targetUserId">ID of the user to follow</param>
    /// <returns>True if follow was successful, false if already following or user not found</returns>
    Task<bool> AddFollowAsync(Guid currentUserId, Guid targetUserId);

    /// <summary>
    /// Removes a follow relationship between two users.
    /// </summary>
    /// <param name="currentUserId">ID of the user who is unfollowing</param>
    /// <param name="targetUserId">ID of the user to unfollow</param>
    /// <returns>True if unfollow was successful, false if relationship not found</returns>
    Task<bool> RemoveFollowAsync(Guid currentUserId, Guid targetUserId);
}
