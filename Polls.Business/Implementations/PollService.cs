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
