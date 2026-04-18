using Dapper;
using Microsoft.Extensions.Configuration;
using Polls.Core.Models.Users;

namespace Polls.DataAccess.DataAccessServices.Implementation;

/// <summary>
/// Example data access service for User entity.
/// Demonstrates how to derive from DataAccessService<T> and implement CRUD operations
/// using specific stored procedures.
/// </summary>
public class UserDataAccessService : DataAccessService<User>
{
    public UserDataAccessService(IConfiguration configuration) : base(configuration)
    {
    }

    public override async Task<User> CreateAsync(User entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id);
        parameters.Add("@Name", entity.Name);
        parameters.Add("@LastName", entity.LastName);
        parameters.Add("@Login", entity.Login);
        parameters.Add("@Password", entity.Password);

        await ExecuteNonQueryProcedureAsync("dbo.CreateUser", parameters);
        return entity;
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        try
        {
            return await ExecuteProcedureAsync("dbo.GetUserById", parameters);
        }
        catch
        {
            return null;
        }
    }

    public override async Task<bool> UpdateAsync(User entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id);
        parameters.Add("@Name", entity.Name);
        parameters.Add("@LastName", entity.LastName);
        parameters.Add("@Login", entity.Login);
        parameters.Add("@Password", entity.Password);

        var rowsAffected = await ExecuteNonQueryProcedureAsync("dbo.UpdateUser", parameters);
        return rowsAffected > 0;
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        var rowsAffected = await ExecuteNonQueryProcedureAsync("dbo.DeleteUser", parameters);
        return rowsAffected > 0;
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;

        var parameters = new DynamicParameters();
        parameters.Add("@Login", login);

        try
        {
            return await ExecuteProcedureAsync("dbo.GetUserByLogin", parameters);
        }
        catch
        {
            return null;
        }
    }
}