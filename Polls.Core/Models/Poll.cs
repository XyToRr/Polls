namespace Polls.Core.Models;

/// <summary>
/// Poll entity matching the Polls table schema.
/// </summary>
public class Poll
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public PollWinnerDecidingAlgorithm Algorithm { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsAnonymous { get; set; }
    public bool? ClosedManually { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Variant> Variants { get; set; } = new();
}
