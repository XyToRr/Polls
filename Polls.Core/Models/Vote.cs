namespace Polls.Core.Models;

/// <summary>
/// Vote entity matching the Votes table schema.
/// </summary>
public class Vote
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsValid { get; set; } = true;
}
