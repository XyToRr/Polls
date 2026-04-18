using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Polls.DataAccess.DataAccessServices.Interfaces;
using System.Data;

namespace Polls.DataAccess.DataAccessServices.Implementation;

/// <summary>
/// Generic base class for data access operations using Dapper ORM with stored procedures.
/// </summary>
/// <typeparam name="T">Entity type matching stored procedure output</typeparam>
public abstract class DataAccessService<T> : IDataAccessService<T> where T : class
{
    protected readonly IConfiguration _configuration;
    protected readonly string _connectionString;

    /// <summary>
    /// Initializes the service with configuration.
    /// </summary>
    /// <param name="configuration">Configuration to retrieve connection string</param>
    protected DataAccessService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration");
    }

    /// <summary>
    /// Creates a new SQL connection.
    /// </summary>
    protected IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Executes a stored procedure and returns the entity result.
    /// </summary>
    /// <param name="procedureName">Name of the stored procedure</param>
    /// <param name="parameters">Dapper DynamicParameters to pass to the procedure</param>
    /// <returns>Entity result from procedure</returns>
    protected async Task<T> ExecuteProcedureAsync(string procedureName, DynamicParameters? parameters = null)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            return await connection.QueryFirstOrDefaultAsync<T>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure) ?? throw new InvalidOperationException($"Procedure {procedureName} returned no result");
        }
    }

    /// <summary>
    /// Executes a stored procedure and returns multiple entity results.
    /// </summary>
    /// <param name="procedureName">Name of the stored procedure</param>
    /// <param name="parameters">Dapper DynamicParameters to pass to the procedure</param>
    /// <returns>IEnumerable of entity results</returns>
    protected async Task<IEnumerable<T>> ExecuteProcedureListAsync(string procedureName, DynamicParameters? parameters = null)
    {
        using (var connection = CreateConnection())
        {
           connection.Open();
            return await connection.QueryAsync<T>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }

    /// <summary>
    /// Executes a stored procedure that returns no result (INSERT/UPDATE/DELETE).
    /// </summary>
    /// <param name="procedureName">Name of the stored procedure</param>
    /// <param name="parameters">Dapper DynamicParameters to pass to the procedure</param>
    /// <returns>Number of rows affected</returns>
    protected async Task<int> ExecuteNonQueryProcedureAsync(string procedureName, DynamicParameters? parameters = null)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            return await connection.ExecuteAsync(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }

    /// <summary>
    /// Creates a new entity using the specified stored procedure.
    /// Derived classes must override and call ExecuteProcedureAsync or ExecuteNonQueryProcedureAsync.
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <returns>The created entity</returns>
    public abstract Task<T> CreateAsync(T entity);

    /// <summary>
    /// Retrieves an entity by its primary key using the specified stored procedure.
    /// Derived classes must override and call ExecuteProcedureAsync.
    /// </summary>
    /// <param name="id">Entity primary key (GUID)</param>
    /// <returns>The entity if found, otherwise null</returns>
    public abstract Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing entity using the specified stored procedure.
    /// Derived classes must override and call ExecuteNonQueryProcedureAsync.
    /// </summary>
    /// <param name="entity">Entity with updated values</param>
    /// <returns>True if update succeeded, false otherwise</returns>
    public abstract Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by its primary key using the specified stored procedure.
    /// Derived classes must override and call ExecuteNonQueryProcedureAsync.
    /// </summary>
    /// <param name="id">Entity primary key (GUID)</param>
    /// <returns>True if delete succeeded, false otherwise</returns>
    public abstract Task<bool> DeleteAsync(Guid id);
}
