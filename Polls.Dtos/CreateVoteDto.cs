namespace Polls.Dtos;

/// <summary>
/// DTO for creating a vote on a poll.
/// </summary>
public class CreateVoteDto
{
    /// <summary>
    /// The ID of the poll being voted on.
    /// </summary>
    public Guid PollId { get; set; }

    /// <summary>
    /// The list of selected variants.
    /// The number of selections and use of Rank field depends on the poll's algorithm.
    /// </summary>
    public List<VoteSelectionDto> SelectedVariants { get; set; } = new();
}
