namespace Polls.Core.Models;

/// <summary>
/// Determines how the winner of a poll is decided.
/// </summary>
public enum PollWinnerDecidingAlgorithm
{
    /// <summary>
    /// The variant with the most votes wins.
    /// </summary>
    MostVotes = 1,

    /// <summary>
    /// Each variant is assigned a rating from 1 to 10.
    /// </summary>
    RatingScale = 2,

    /// <summary>
    /// Variants are ranked from best to worst.
    /// </summary>
    Ranking = 3
}
