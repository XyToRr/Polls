namespace Polls.Core.Models;

/// <summary>
/// Represents a single vote selection returned from GetPollResults stored procedure.
/// </summary>
public class SelectionResult
{
    /// <summary>
    /// The variant ID.
    /// </summary>
    public Guid VariantId { get; set; }

    /// <summary>
    /// The variant text/description.
    /// </summary>
    public string VariantText { get; set; } = string.Empty;

    /// <summary>
    /// The vote ID (identifies individual votes).
    /// Can be null if variant received no votes.
    /// </summary>
    public Guid? VoteId { get; set; }

    /// <summary>
    /// The rank value assigned in the vote (ignored for MostVotes algorithm).
    /// </summary>
    public int? Rank { get; set; }
}
