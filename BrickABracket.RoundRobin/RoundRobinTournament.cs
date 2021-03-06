﻿using System.Collections.Generic;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.RoundRobin
{
    public class RoundRobinTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 2;
        public int GenerateRound(Category category, int roundIndex = -1, int runoff = 0)
        {
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
