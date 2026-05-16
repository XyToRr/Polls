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

    /// <summary>
    /// Gets all poll results sorted from best to worst performance.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <returns>Poll results DTO with all variants sorted</returns>
    /// <exception cref="ArgumentException">Thrown when poll not found</exception>
    Task<PollResultsDto> GetPollResultsAsync(Guid pollId);

    /// <summary>
    /// Gets only the winner of a poll.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <returns>Winning variant result</returns>
    /// <exception cref="ArgumentException">Thrown when poll not found</exception>
    /// <exception cref="InvalidOperationException">Thrown when poll has no votes</exception>
    Task<VariantResultDto> GetPollWinnerAsync(Guid pollId);

    /// <summary>
    /// Bans a user from a poll.
    /// </summary>
    /// <param name="pollId">ID of the poll</param>
    /// <param name="ownerUserId">ID of the poll owner (must be the current user)</param>
    /// <param name="targetUserId">ID of the user to ban</param>
    /// <returns>True if ban was successful, false if user already banned or not found</returns>
    /// <exception cref="ArgumentException">Thrown when poll not found or validation fails</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not the poll owner</exception>
    Task<bool> BanUserAsync(Guid pollId, Guid ownerUserId, Guid targetUserId);

    /// <summary>
    /// Unbans a user from a poll.
    /// </summary>
    /// <param name="pollId">ID of the poll</param>
    /// <param name="ownerUserId">ID of the poll owner (must be the current user)</param>
    /// <param name="targetUserId">ID of the user to unban</param>
    /// <returns>True if unban was successful, false if user not banned</returns>
    /// <exception cref="ArgumentException">Thrown when poll not found or validation fails</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not the poll owner</exception>
    Task<bool> UnbanUserAsync(Guid pollId, Guid ownerUserId, Guid targetUserId);
}
