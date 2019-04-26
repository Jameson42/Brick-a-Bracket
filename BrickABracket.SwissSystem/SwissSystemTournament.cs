using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SwissSystem
{
    public class SwissSystemTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 2;
    }
}
