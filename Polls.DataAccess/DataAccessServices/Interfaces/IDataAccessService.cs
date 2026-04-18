namespace Polls.DataAccess.DataAccessServices.Interfaces;

/// <summary>
/// Generic interface for data access operations on entities of type T.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IDataAccessService<T> where T : class
{
    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <returns>The created entity or affected rows</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Retrieves an entity by its primary key.
    /// </summary>
    /// <param name="id">Entity primary key (GUID)</param>
    /// <returns>The entity if found, otherwise null</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">Entity with updated values</param>
    /// <returns>True if update succeeded, false otherwise</returns>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by its primary key.
    /// </summary>
    /// <param name="id">Entity primary key (GUID)</param>
    /// <returns>True if delete succeeded, false otherwise</returns>
    Task<bool> DeleteAsync(Guid id);
}
