using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.RoundRobin
{
    public class RoundRobinTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 2;
    }
}
