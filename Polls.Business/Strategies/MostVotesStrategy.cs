using Polls.Core.Models;

namespace Polls.Business.Strategies;

/// <summary>
/// Strategy for determining winner in MostVotes polls.
/// The winner is the variant with the most votes.
/// </summary>
public class MostVotesStrategy : IWinnerStrategy
{
    /// <summary>
    /// Determines the winner based on vote count.
    /// </summary>
    public VariantResult? DetermineWinner(List<SelectionResult> selections)
    {
        if (selections == null || selections.Count == 0)
            return null;

        var results = GetSortedResults(selections);
        return results.FirstOrDefault();
    }

    /// <summary>
    /// Sorts variants by vote count (descending).
    /// Variants with no votes (null VoteId) are placed at the end.
    /// </summary>
    public List<VariantResult> GetSortedResults(List<SelectionResult> selections)
    {
        return selections
            .GroupBy(s => new { s.VariantId, s.VariantText })
            .Select(g => new VariantResult
            {
                VariantId = g.Key.VariantId,
                VariantText = g.Key.VariantText,
                Score = g.Where(s => s.VoteId != null).Select(s => s.VoteId).Distinct().Count()
            })
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.VariantId) // Secondary sort for consistent ordering
            .ToList();
    }
}
