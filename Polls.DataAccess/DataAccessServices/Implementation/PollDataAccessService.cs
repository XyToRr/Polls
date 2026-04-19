using Dapper;
using Microsoft.Extensions.Configuration;
using Polls.Core.Models;
using System.Data;

namespace Polls.DataAccess.DataAccessServices.Implementation;

/// <summary>
/// Data access service for Poll entity.
/// Provides methods to interact with polls and variants in the database.
/// </summary>
public class PollDataAccessService : DataAccessService<Poll>
{
    public PollDataAccessService(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Creates a poll with variants using the CreatePollWithVariants stored procedure.
    /// </summary>
    /// <param name="poll">The poll to create</param>
    /// <param name="variants">List of variants for the poll</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> CreatePollWithVariantsAsync(Poll poll, List<Variant> variants)
    {
        if (poll == null)
            throw new ArgumentNullException(nameof(poll));

        if (variants == null || variants.Count == 0)
            throw new ArgumentException("At least one variant is required", nameof(variants));

        try
        {
            // Create a DataTable for variants
            var variantsTable = new DataTable();
            variantsTable.Columns.Add("Id", typeof(Guid));
            variantsTable.Columns.Add("Text", typeof(string));

            foreach (var variant in variants)
            {
                variantsTable.Rows.Add(variant.Id, variant.Text);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Id", poll.Id);
            parameters.Add("@Title", poll.Title);
            parameters.Add("@Description", poll.Description);
            parameters.Add("@OwnerUserId", poll.OwnerUserId);
            parameters.Add("@Algorithm", (int)poll.Algorithm);
            parameters.Add("@StartDate", poll.StartDate);
            parameters.Add("@EndDate", poll.EndDate);
            parameters.Add("@IsAnonymous", poll.IsAnonymous);
            parameters.Add("@Variants", variantsTable.AsTableValuedParameter("dbo.VariantList"));

            var result = await ExecuteNonQueryProcedureAsync("dbo.CreatePollWithVariants", parameters);
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create poll with variants", ex);
        }
    }

    /// <summary>
    /// Creates a new poll using the standard CreatePoll stored procedure.
    /// Note: For creating polls with variants, use CreatePollWithVariantsAsync instead.
    /// </summary>
    /// <param name="entity">Poll entity to create</param>
    /// <returns>The created poll</returns>
    public override async Task<Poll> CreateAsync(Poll entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id);
        parameters.Add("@Title", entity.Title);
        parameters.Add("@Description", entity.Description);
        parameters.Add("@OwnerUserId", entity.OwnerUserId);
        parameters.Add("@Algorithm", (int)entity.Algorithm);
        parameters.Add("@StartDate", entity.StartDate);
        parameters.Add("@EndDate", entity.EndDate);
        parameters.Add("@IsAnonymous", entity.IsAnonymous);

        await ExecuteNonQueryProcedureAsync("dbo.CreatePoll", parameters);
        return entity;
    }

    /// <summary>
    /// Retrieves a poll by its ID.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>Poll if found, otherwise null</returns>
    public override async Task<Poll?> GetByIdAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        try
        {
            return await ExecuteProcedureAsync("dbo.GetPollById", parameters);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Updates an existing poll.
    /// </summary>
    /// <param name="entity">Poll with updated values</param>
    /// <returns>True if update succeeded, false otherwise</returns>
    public override async Task<bool> UpdateAsync(Poll entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var parameters = new DynamicParameters();
        parameters.Add("@Id", entity.Id);
        parameters.Add("@Title", entity.Title);
        parameters.Add("@Description", entity.Description);
        parameters.Add("@Algorithm", (int)entity.Algorithm);
        parameters.Add("@EndDate", entity.EndDate);
        parameters.Add("@IsAnonymous", entity.IsAnonymous);

        var rowsAffected = await ExecuteNonQueryProcedureAsync("dbo.UpdatePoll", parameters);
        return rowsAffected > 0;
    }

    /// <summary>
    /// Deletes a poll by its ID.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>True if delete succeeded, false otherwise</returns>
    public override async Task<bool> DeleteAsync(Guid id)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        var rowsAffected = await ExecuteNonQueryProcedureAsync("dbo.DeletePoll", parameters);
        return rowsAffected > 0;
    }
}
