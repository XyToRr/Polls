namespace Polls.Dtos;

/// <summary>
/// DTO for creating a variant within a poll.
/// </summary>
public class CreateVariantDto
{
    /// <summary>
    /// The text of the variant.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
