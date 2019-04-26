using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SingleElimination
{
    public class SingleEliminationTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 2;
    }
}
