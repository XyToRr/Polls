using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polls.Business.Interfaces;
using Polls.Core.Models;
using Polls.Dtos;
using System.Security.Claims;

namespace Polls.Controllers;

/// <summary>
/// Controller for managing polls.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly IPollService _pollService;
    private readonly ILogger<PollsController> _logger;

    public PollsController(IPollService pollService, ILogger<PollsController> logger)
    {
        _pollService = pollService ?? throw new ArgumentNullException(nameof(pollService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new poll with variants.
    /// </summary>
    /// <param name="dto">Poll creation data</param>
    /// <returns>Created poll with 201 status code</returns>
    /// <response code="201">Poll created successfully</response>
    /// <response code="400">Invalid poll data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("create-with-variants")]
    public async Task<ActionResult> CreatePollWithVariants([FromBody] CreatePollWithVariantsDto dto)
    {
        if (dto == null)
            return BadRequest("Poll data is required");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Get the user ID from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Failed to extract user ID from JWT token");
                return Unauthorized("Invalid user context");
            }

            var poll = await _pollService.CreatePollWithVariantsAsync(dto, userId);

            return CreatedAtAction(nameof(GetPoll), new { id = poll.Id }, poll);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating poll");
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation error creating poll");
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating poll");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Retrieves a poll by its ID.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>Poll data if found</returns>
    /// <response code="200">Poll retrieved successfully</response>
    /// <response code="404">Poll not found</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetPoll(Guid id)
    {
        try
        {
            var poll = await _pollService.GetPollByIdAsync(id);

            if (poll == null)
                return NotFound(new { error = "Poll not found" });

            return Ok(poll);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving poll {PollId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the poll" });
        }
    }

    /// <summary>
    /// Adds a vote to a poll.
    /// </summary>
    /// <param name="dto">Vote creation data</param>
    /// <returns>Created vote with 201 status code</returns>
    /// <response code="201">Vote created successfully</response>
    /// <response code="400">Invalid vote data or poll validation failed</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("vote")]
    public async Task<ActionResult> AddVote([FromBody] CreateVoteDto dto)
    {
        if (dto == null)
            return BadRequest("Vote data is required");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Get the user ID from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Failed to extract user ID from JWT token");
                return Unauthorized("Invalid user context");
            }

            var vote = await _pollService.CreateVoteAsync(dto, userId);

            return CreatedAtAction(nameof(GetPoll), new { id = vote.PollId }, vote);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating vote");
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation error creating vote");
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating vote");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Retrieves poll results with all variants sorted from best to worst.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>Poll results with sorted variants</returns>
    /// <response code="200">Results retrieved successfully</response>
    /// <response code="404">Poll not found</response>
    /// <response code="400">Poll has no votes</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}/results")]
    public async Task<ActionResult> GetPollResults(Guid id)
    {
        try
        {
            var results = await _pollService.GetPollResultsAsync(id);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Poll not found {PollId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operation error retrieving results for {PollId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving results for {PollId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving poll results" });
        }
    }

    /// <summary>
    /// Retrieves only the winner of a poll.
    /// </summary>
    /// <param name="id">Poll ID</param>
    /// <returns>Winning variant</returns>
    /// <response code="200">Winner retrieved successfully</response>
    /// <response code="404">Poll not found</response>
    /// <response code="400">Poll has no votes</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}/winner")]
    public async Task<ActionResult> GetPollWinner(Guid id)
    {
        try
        {
            var winner = await _pollService.GetPollWinnerAsync(id);
            return Ok(winner);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Poll not found {PollId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operation error retrieving winner for {PollId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving winner for {PollId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving poll winner" });
        }
    }
}
