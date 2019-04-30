﻿using System.Collections.Generic;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SwissSystem
{
    public class SwissSystemTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 2;
        public int GenerateMatch(int categoryIndex, int roundIndex = -1, int matchIndex = -1)
        {
            return -1;
        }
        public bool GenerateCategoryStandings(int categoryIndex)
        {
            return false;
        }
        public bool GenerateRoundStandings(int categoryIndex, int roundIndex)
        {
            return false;
        }
    }
}
