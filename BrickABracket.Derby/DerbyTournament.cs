using System;
using System.Collections.Generic;
using System.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Derby
{
    public class DerbyTournament: ITournamentStrategy
    {
        private const int matchSize = 4;
        private const string patternPrefix = "derby";
        private Tournament _tournament; 
        private TournamentService _tournamentService;
        private MocService _mocService;
        private CompetitorService _competitorService;
        private PatternService _patternService;
        public DerbyTournament(TournamentService tournamentService, 
            MocService mocService, CompetitorService competitorService, 
            PatternService patternService, int tournamentId)
        {
            _tournamentService = tournamentService;
            _mocService = mocService;
            _competitorService = competitorService;
            _patternService = patternService;
            _tournament = _tournamentService.Read(tournamentId);
        }

        // TODO: Add methods for "next round", "next match", etc?

        private TournamentPattern GetPattern(int size, bool forceGenerate = false)
        {
            if (size<1)
                return null;
            string id = PatternId(size);
            if (!forceGenerate)
                try
                {
                    return _patternService.Read(id);
                }
                catch // Read throws on not found
                {}

            var pattern = GeneratePattern(size);
            _patternService.Upsert(pattern);
            return pattern;
        }

        private TournamentPattern GeneratePattern(int size)
        {
            // If not enough players for match size, default to match size
            if (size<matchSize)
                return GetPattern(matchSize);
            var tournamentPattern = new TournamentPattern();
            tournamentPattern._id = PatternId(size);


            // It doesn't make sense for a derby to have multiple rounds for most
            // player counts (any not devisible by matchSize)
            var round = new RoundPattern();
            for (int i=0;i<size;i++)
            {
                var match = new MatchPattern();
                for (int j=0;j<matchSize;j++)
                {
                    var pickList = Enumerable.Range(0,size)
                        .Where(p => !match.Mocs.Contains(p));

                    // If pickList is empty, we've reached a bad 
                    // state due to pickList randomness. 
                    // Restart the selection for the current match.
                    if (!pickList.Any())
                    {
                        match = new MatchPattern();
                        j = -1;
                        continue;
                    }

                    var matchCounts = round.Matches
                        .SelectMany(p => p.Mocs)
                        .GroupBy(m => m)
                        .Select(m => new { Count = m.Count(), Moc = m.Key });
                    var fewestMatches = matchCounts
                        .Select(m => m.Count)
                        .Min();
                    var availableMocs = matchCounts
                        .Where(m => m.Count == fewestMatches)
                        .Select(m => m.Moc);

                    pickList = pickList
                        .Intersect(availableMocs)
                        .OrderBy(p => FacingCount(round, match, p))
                        .ThenBy(p => System.Guid.NewGuid()); // Random

                    if (fewestMatches > 0)
                        pickList = pickList
                            .Where(m => !round.Matches.Last().Mocs.Contains(m));

                    match.Mocs.Add(pickList.Count()>0
                        ? pickList.First()
                        : -1);
                }
                round.Matches.Add(match);
            }

            tournamentPattern.Rounds.Add(round);
            return tournamentPattern;
        }
        
        /// <summary>
        /// Find how many times id has faced other mocs from the current match
        /// </summary>
        /// <param name="round">The round</param>
        /// <param name="match">The current match</param>
        /// <param name="id">id to check</param>
        /// <returns>Total</returns>
        private static int FacingCount(RoundPattern round, MatchPattern match, int id)
        {
            return round.Matches
                .Where(m => m.Mocs.Contains(id))
                .Where(m => match.Mocs.Any(i => m.Mocs.Contains(i)))
                .Count();
        }

        private string PatternId(int size) => $"{patternPrefix}{size}";
    }
}
