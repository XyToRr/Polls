using BCrypt.Net;
using Polls.Business.Interfaces;

namespace Polls.Business.Implementations;

/// <summary>
/// Password hashing service using BCrypt.
/// </summary>
public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
