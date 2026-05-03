using Polls.Core.Models;

namespace Polls.Business.Strategies;

/// <summary>
/// Strategy interface for determining poll winners based on algorithm type.
/// </summary>
public interface IWinnerStrategy
{
    /// <summary>
    /// Determines the winner from selection results.
    /// </summary>
    /// <param name="selections">Raw selection data from GetPollResults procedure</param>
    /// <returns>The winning variant result (with highest score), or null if no selections</returns>
    VariantResult? DetermineWinner(List<SelectionResult> selections);

    /// <summary>
    /// Sorts variants from best to worst performance based on voting results.
    /// </summary>
    /// <param name="selections">Raw selection data for calculating metrics</param>
    /// <returns>Sorted list of variants with scores (best to worst)</returns>
    List<VariantResult> GetSortedResults(List<SelectionResult> selections);
}
