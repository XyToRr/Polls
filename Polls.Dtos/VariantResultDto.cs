namespace Polls.Dtos;

/// <summary>
/// DTO for a single variant result in poll results.
/// </summary>
public class VariantResultDto
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
    /// The calculated score (vote count for MostVotes, average rank for others).
    /// </summary>
    public double Score { get; set; }
}
