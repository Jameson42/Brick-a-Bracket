using System.Collections.Generic;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SingleElimination
{
    public class SingleEliminationTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 2;
        public int GenerateRound(Category category, int roundIndex = -1, int runoff = 0)
        {
            // If index == -1, give last incomplete round or create next
            // If round 0, create from category MOCs
            // Else create from winners of previous round
            return -1;
        }
        private int FirstUnfinishedRound(Category category)
        {
            if (category == null)
                return -1;
            for (int i=0; i<category.Rounds.Count; i++)
            {
                var round = category.Rounds[i];
                // TODO: Verify if all matches created and have results
            }
            return -1;
        }
        public int GenerateMatch(Round round, int matchIndex = -1)
        {
            return -1;
        }
        public bool GenerateCategoryStandings(Category category)
        {
            return false;
        }
        public bool GenerateRoundStandings(Round round)
        {
            return false;
        }
    }
}
