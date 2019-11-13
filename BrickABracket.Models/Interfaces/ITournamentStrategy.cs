using BrickABracket.Models.Base;
namespace BrickABracket.Models.Interfaces
{
    public interface ITournamentStrategy
    {
        int MatchSize { get; }
        /// <summary>
        /// Generates a round within the given category. 
        /// Defaults to next unfinished round if no round index specified.
        /// Entering an existing round index recreates that round.
        /// Entering a runoff value creates a new round using
        /// that many top MOCs from the previous round.
        /// </summary>
        /// <returns>Returns round index, or -1 on a failure</returns>
        int GenerateRound(Category category, int roundIndex = -1, int runoff = 0);
        /// <summary>
        /// Generates a match within the given round. 
        /// Defaults to next match if no match index or 
        /// round index specified.
        /// Entering an existing match index recreates that match 
        /// at the end of the match list.
        /// </summary>
        /// <returns>Returns new match index, or -1 on a failure</returns>
        int GenerateMatch(Round round, int matchIndex = -1);
        bool GenerateCategoryStandings(Category category);
        bool GenerateRoundStandings(Round round);
    }
}