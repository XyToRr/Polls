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

    /// <summary>
    /// Checks if a user is banned from voting on a specific poll.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>True if user is banned, false otherwise</returns>
    public async Task<bool> IsUserBannedAsync(Guid pollId, Guid userId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PollId", pollId);
        parameters.Add("@UserId", userId);

        try
        {
            var result = await ExecuteProcedureAsync<bool?>("dbo.CheckUserIsBanned", parameters);
            return result ?? false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a user has already voted on a specific poll.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>True if user has already voted, false otherwise</returns>
    public async Task<bool> HasUserVotedAsync(Guid pollId, Guid userId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PollId", pollId);
        parameters.Add("@UserId", userId);

        try
        {
            var result = await ExecuteProcedureAsync<bool?>("dbo.CheckUserHasVoted", parameters);
            return result ?? false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a vote using the CreateVote stored procedure.
    /// </summary>
    /// <param name="voteId">Vote ID (new GUID)</param>
    /// <param name="pollId">Poll ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="selections">List of variant selections with optional ranking</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> CreateVoteAsync(Guid voteId, Guid pollId, Guid userId, List<(Guid VariantId, int? Rank)> selections)
    {
        if (selections == null || selections.Count == 0)
            throw new ArgumentException("At least one variant selection is required", nameof(selections));

        try
        {
            var selectionsTable = new DataTable();
            selectionsTable.Columns.Add("VariantId", typeof(Guid));
            selectionsTable.Columns.Add("Rank", typeof(int));

            foreach (var (variantId, rank) in selections)
            {
                selectionsTable.Rows.Add(variantId, (object?)rank ?? DBNull.Value);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Id", voteId);
            parameters.Add("@PollId", pollId);
            parameters.Add("@UserId", userId);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@Selections", selectionsTable.AsTableValuedParameter("dbo.VoteSelectionList"));

            var result = await ExecuteNonQueryProcedureAsync("dbo.CreateVote", parameters);
            return result >= 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create vote", ex);
        }
    }

    /// <summary>
    /// Retrieves variants for a given poll.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <returns>List of variants belonging to the poll</returns>
    public async Task<List<Variant>> GetVariantsByPollIdAsync(Guid pollId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PollId", pollId);

        try
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var rows = await connection.QueryAsync<Variant>(
                    "dbo.GetVariantsByPollId",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return rows.ToList();
            }
        }
        catch
        {
            return new List<Variant>();
        }
    }

    /// <summary>
    /// Retrieves poll results using the GetPollResults stored procedure.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <returns>List of selection results (variant id, text, vote id, rank)</returns>
    public async Task<List<SelectionResult>> GetPollResultsAsync(Guid pollId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PollId", pollId);

        try
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var rows = await connection.QueryAsync<SelectionResult>(
                    "dbo.GetPollResults",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return rows.ToList();
            }
        }
        catch
        {
            return new List<SelectionResult>();
        }
    }
}