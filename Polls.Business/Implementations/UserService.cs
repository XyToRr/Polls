using Polls.Business.Interfaces;
using Polls.Core.Models.Users;
using Polls.DataAccess;
using Polls.DataAccess.DataAccessServices.Implementation;

namespace Polls.Business.Implementations;

/// <summary>
/// User management service.
/// </summary>
public class UserService : IUserService
{
    private readonly UserDataAccessService _userDataAccess;

    public UserService(UserDataAccessService userDataAccess)
    {
        _userDataAccess = userDataAccess ?? throw new ArgumentNullException(nameof(userDataAccess));
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;

        return await _userDataAccess.GetByLoginAsync(login);
    }

    public async Task<User> CreateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return await _userDataAccess.CreateAsync(user);
    }
}
