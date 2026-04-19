namespace Polls.Dtos;

/// <summary>
/// DTO for creating a poll with variants.
/// </summary>
public class CreatePollWithVariantsDto
{
    /// <summary>
    /// The title of the poll.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the poll (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The algorithm for determining the winner (1, 2, or 3).
    /// </summary>
    public int Algorithm { get; set; }

    /// <summary>
    /// The start date/time of the poll.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date/time of the poll (optional).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Whether the poll is anonymous.
    /// </summary>
    public bool IsAnonymous { get; set; } = false;

    /// <summary>
    /// The variants/options for the poll.
    /// </summary>
    public List<CreateVariantDto> Variants { get; set; } = new();
}
