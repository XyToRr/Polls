using Polls.Core.Models;

namespace Polls.Business.Strategies;

/// <summary>
/// Strategy for determining winner in RatingScale polls.
/// The winner is the variant with the best (highest) average rank.
/// </summary>
public class RatingScaleStrategy : IWinnerStrategy
{
    /// <summary>
    /// Determines the winner based on highest average rank.
    /// </summary>
    public VariantResult? DetermineWinner(List<SelectionResult> selections)
    {
        if (selections == null || selections.Count == 0)
            return null;

        var results = GetSortedResults(selections);
        return results.FirstOrDefault();
    }

    /// <summary>
    /// Sorts variants by average rank (descending - higher is better).
    /// Variants with no votes (null VoteId) are placed at the end.
    /// </summary>
    public List<VariantResult> GetSortedResults(List<SelectionResult> selections)
    {
        var variantsWithVotes = selections
            .Where(s => s.VoteId != null && s.Rank.HasValue)
            .GroupBy(s => new { s.VariantId, s.VariantText })
            .Select(g => new VariantResult
            {
                VariantId = g.Key.VariantId,
                VariantText = g.Key.VariantText,
                Score = g.Average(s => s.Rank ?? 0)
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        var variantsWithoutVotes = selections
            .Where(s => s.VoteId == null)
            .GroupBy(s => new { s.VariantId, s.VariantText })
            .Select(g => new VariantResult
            {
                VariantId = g.Key.VariantId,
                VariantText = g.Key.VariantText,
                Score = 0
            })
            .ToList();

        return variantsWithVotes.Concat(variantsWithoutVotes).ToList();
    }
}
