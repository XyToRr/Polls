using Polls.Business.Interfaces;
using Polls.Core.Models;
using Polls.DataAccess.DataAccessServices.Implementation;
using Polls.Dtos;

namespace Polls.Business.Implementations;

/// <summary>
/// Poll service for managing poll creation and retrieval.
/// </summary>
public class PollService : IPollService
{
    private readonly PollDataAccessService _pollDataAccessService;

    public PollService(PollDataAccessService pollDataAccessService)
    {
        _pollDataAccessService = pollDataAccessService ?? throw new ArgumentNullException(nameof(pollDataAccessService));
    }

    /// <summary>
    /// Creates a new poll with variants.
    /// </summary>
    /// <param name="dto">Poll creation data</param>
    /// <param name="ownerUserId">ID of the user creating the poll</param>
    /// <returns>The created poll</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are missing or invalid</exception>
    public async Task<Poll> CreatePollWithVariantsAsync(CreatePollWithVariantsDto dto, Guid ownerUserId)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        ValidateCreatePollRequest(dto);

        var pollId = Guid.NewGuid();
        var variants = dto.Variants.Select(v => new Variant
        {
            Id = Guid.NewGuid(),
            PollId = pollId,
            Text = v.Text
        }).ToList();

        var poll = new Poll
        {
            Id = pollId,
            Title = dto.Title,
            Description = dto.Description,
            OwnerUserId = ownerUserId,
            Algorithm = (PollWinnerDecidingAlgorithm)dto.Algorithm,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsAnonymous = dto.IsAnonymous,
            CreatedAt = DateTime.UtcNow,
            Variants = variants
        };

        var success = await _pollDataAccessService.CreatePollWithVariantsAsync(poll, variants);

        if (!success)
            throw new InvalidOperationException("Failed to create poll in the database");

        return poll;
    }

    /// <summary>
    /// Retrieves a poll by its ID.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>Poll if found, otherwise null</returns>
    public async Task<Poll?> GetPollByIdAsync(Guid id)
    {
        return await _pollDataAccessService.GetByIdAsync(id);
    }

    /// <summary>
    /// Creates a vote on a poll with validation.
    /// </summary>
    /// <param name="dto">Vote creation data</param>
    /// <param name="userId">ID of the user voting</param>
    /// <returns>The created vote</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when operation fails</exception>
    public async Task<Vote> CreateVoteAsync(CreateVoteDto dto, Guid userId)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        ValidateCreateVoteRequest(dto);

        // Get the poll
        var poll = await GetPollByIdAsync(dto.PollId);
        if (poll == null)
            throw new ArgumentException("Poll not found", nameof(dto.PollId));

        // Ensure we have variants for validation. Some stored procedures may not return variants with the poll.
        if (poll.Variants == null || poll.Variants.Count == 0)
        {
            var variants = await _pollDataAccessService.GetVariantsByPollIdAsync(dto.PollId);
            poll.Variants = variants ?? new List<Variant>();
        }

        // Check if poll is closed manually
        if (poll.ClosedManually == true)
            throw new ArgumentException("This poll is closed", nameof(dto.PollId));

        // Check if poll is within the open period
        var now = DateTime.UtcNow;
        if (now < poll.StartDate)
            throw new ArgumentException("This poll has not started yet", nameof(dto.PollId));

        if (poll.EndDate.HasValue && now > poll.EndDate)
            throw new ArgumentException("This poll has ended", nameof(dto.PollId));

        // Check if user is banned
        if (await _pollDataAccessService.IsUserBannedAsync(dto.PollId, userId))
            throw new ArgumentException("You are banned from voting on this poll", nameof(dto.PollId));

        // Check if user has already voted
        if (await _pollDataAccessService.HasUserVotedAsync(dto.PollId, userId))
            throw new ArgumentException("You have already voted on this poll", nameof(dto.PollId));

        // Validate selected variants belong to this poll
        var selectedVariantIds = dto.SelectedVariants.Select(v => v.VariantId).ToHashSet();
        var pollVariantIds = poll.Variants.Select(v => v.Id).ToHashSet();

        foreach (var variantId in selectedVariantIds)
        {
            if (!pollVariantIds.Contains(variantId))
                throw new ArgumentException("One or more selected variants do not belong to this poll", nameof(dto.SelectedVariants));
        }

        // Validate based on algorithm
        ValidateVoteByAlgorithm(dto, poll.Algorithm);

        // Create the vote
        var voteId = Guid.NewGuid();
        var selections = dto.SelectedVariants
            .Select(v => (v.VariantId, v.Rank))
            .ToList();

        var success = await _pollDataAccessService.CreateVoteAsync(voteId, dto.PollId, userId, selections);

        if (!success)
            throw new InvalidOperationException("Failed to create vote in the database");

        return new Vote
        {
            Id = voteId,
            PollId = dto.PollId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsValid = true
        };
    }

    /// <summary>
    /// Validates the create vote request data.
    /// </summary>
    /// <param name="dto">Vote creation DTO</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    private static void ValidateCreateVoteRequest(CreateVoteDto dto)
    {
        if (dto.PollId == Guid.Empty)
            throw new ArgumentException("Poll ID is required", nameof(dto.PollId));

        if (dto.SelectedVariants == null || dto.SelectedVariants.Count == 0)
            throw new ArgumentException("At least one variant must be selected", nameof(dto.SelectedVariants));
    }

    /// <summary>
    /// Validates the vote based on the poll's algorithm.
    /// </summary>
    /// <param name="dto">Vote creation DTO</param>
    /// <param name="algorithm">Poll algorithm type</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    private static void ValidateVoteByAlgorithm(CreateVoteDto dto, PollWinnerDecidingAlgorithm algorithm)
    {
        switch (algorithm)
        {
            case PollWinnerDecidingAlgorithm.MostVotes:
                // Only one selection allowed
                if (dto.SelectedVariants.Count != 1)
                    throw new ArgumentException("This poll only allows selecting one variant", nameof(dto.SelectedVariants));
                break;

            case PollWinnerDecidingAlgorithm.RatingScale:
                // Multiple selections allowed, Rank field ignored
                // No additional validation needed
                break;

            case PollWinnerDecidingAlgorithm.Ranking:
                // Multiple selections with ranking required
                // All selections must have unique, valid ranks
                var ranks = dto.SelectedVariants
                    .Where(v => v.Rank.HasValue)
                    .Select(v => v.Rank.Value)
                    .ToList();

                if (ranks.Count != dto.SelectedVariants.Count)
                    throw new ArgumentException("Ranked voting requires all variants to have a rank value", nameof(dto.SelectedVariants));

                // Check for duplicate ranks
                if (ranks.Distinct().Count() != ranks.Count)
                    throw new ArgumentException("All variant ranks must be unique", nameof(dto.SelectedVariants));

                // Check that ranks start from 1 and are consecutive
                var sortedRanks = ranks.OrderBy(r => r).ToList();
                for (int i = 0; i < sortedRanks.Count; i++)
                {
                    if (sortedRanks[i] != i + 1)
                        throw new ArgumentException("Ranks must be consecutive starting from 1", nameof(dto.SelectedVariants));
                }
                break;
        }
    }

    /// <summary>
    /// Validates the create poll request data.
    /// </summary>
    /// <param name="dto">Poll creation DTO</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    private static void ValidateCreatePollRequest(CreatePollWithVariantsDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Poll title is required", nameof(dto.Title));

        if (dto.Title.Length > 50)
            throw new ArgumentException("Poll title cannot exceed 50 characters", nameof(dto.Title));

        if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 250)
            throw new ArgumentException("Poll description cannot exceed 250 characters", nameof(dto.Description));

        if (dto.Algorithm < 1 || dto.Algorithm > 3)
            throw new ArgumentException("Algorithm must be 1, 2, or 3", nameof(dto.Algorithm));

        if (dto.StartDate == default)
            throw new ArgumentException("Start date is required", nameof(dto.StartDate));

        if (dto.EndDate.HasValue && dto.EndDate <= dto.StartDate)
            throw new ArgumentException("End date must be after start date", nameof(dto.EndDate));

        if (dto.Variants == null || dto.Variants.Count == 0)
            throw new ArgumentException("At least one variant is required", nameof(dto.Variants));

        foreach (var variant in dto.Variants)
        {
            if (string.IsNullOrWhiteSpace(variant.Text))
                throw new ArgumentException("Variant text cannot be empty", nameof(variant.Text));

            if (variant.Text.Length > 50)
                throw new ArgumentException("Variant text cannot exceed 50 characters", nameof(variant.Text));
        }
    }
}
