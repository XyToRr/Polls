using Polls.Core.Models;
using Polls.Dtos;

namespace Polls.Business.Interfaces;

/// <summary>
/// Interface for poll service.
/// </summary>
public interface IPollService
{
    /// <summary>
    /// Creates a new poll with variants.
    /// </summary>
    /// <param name="dto">Poll creation data</param>
    /// <param name="ownerUserId">ID of the user creating the poll</param>
    /// <returns>The created poll</returns>
    Task<Poll> CreatePollWithVariantsAsync(CreatePollWithVariantsDto dto, Guid ownerUserId);

    /// <summary>
    /// Retrieves a poll by its ID.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>Poll if found, otherwise null</returns>
    Task<Poll?> GetPollByIdAsync(Guid id);

    /// <summary>
    /// Creates a vote on a poll with validation.
    /// </summary>
    /// <param name="dto">Vote creation data</param>
    /// <param name="userId">ID of the user voting</param>
    /// <returns>The created vote</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when operation fails</exception>
    Task<Vote> CreateVoteAsync(CreateVoteDto dto, Guid userId);
}
