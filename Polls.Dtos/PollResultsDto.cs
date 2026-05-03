namespace Polls.Dtos;

/// <summary>
/// DTO for poll results containing all variants sorted by performance.
/// </summary>
public class PollResultsDto
{
    /// <summary>
    /// Poll ID.
    /// </summary>
    public Guid PollId { get; set; }

    /// <summary>
    /// Poll title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Variants sorted from best to worst performance.
    /// </summary>
    public List<VariantResultDto> Results { get; set; } = new();
}
