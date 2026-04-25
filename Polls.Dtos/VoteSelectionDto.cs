namespace Polls.Dtos;

/// <summary>
/// DTO for a single variant selection within a vote.
/// </summary>
public class VoteSelectionDto
{
    /// <summary>
    /// The ID of the selected variant.
    /// </summary>
    public Guid VariantId { get; set; }

    /// <summary>
    /// The rank of this variant (only used for ranked voting algorithms).
    /// </summary>
    public int? Rank { get; set; }
}
