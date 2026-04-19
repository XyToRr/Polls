namespace Polls.Core.Models;

/// <summary>
/// Variant entity matching the Variants table schema.
/// </summary>
public class Variant
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public string Text { get; set; } = string.Empty;
}
