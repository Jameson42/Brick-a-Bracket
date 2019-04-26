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
        public int MatchSize {get;} = 4;
        private const string patternPrefix = "derby";
        private Tournament _tournament; 
        private TournamentService _tournamentService;
        private MocService _mocService;
        private CompetitorService _competitorService;
        public DerbyTournament(TournamentService tournamentService, 
            MocService mocService, CompetitorService competitorService, 
            Tournament tournament)
        {
            _tournamentService = tournamentService;
            _mocService = mocService;
            _competitorService = competitorService;
            _tournament = tournament;
        }
        public DerbyTournament(TournamentService tournamentService, 
            MocService mocService, CompetitorService competitorService, 
            int tournamentId):this(tournamentService, mocService, competitorService, tournamentService.Read(tournamentId))
        {}

        // TODO: Add methods for "next round", "next match", etc?

        /// <summary>
        /// Generates a match within the given round. 
        /// Defaults to next match if no match number is specified
        /// </summary>
        /// <returns>Returns new match number, or -1 on a failure</returns>
        private int GenerateMatch(Category category, Round round, int matchNumber = -1)
        {
            if (matchNumber>category.MocIds.Count)
                return -1;
            if (matchNumber<0)
                matchNumber = round.Matches.Count;
            if (round.Matches.Count > matchNumber)
            {
                round.Matches.RemoveAt(matchNumber);
                matchNumber = round.Matches.Count;
            }
            var match = new Match();
            for (int i=0;i<MatchSize;i++)
                {
                    var pickList = category.MocIds
                        .Where(p => !match.MocIds.Contains(p));

                    // If pickList is empty, we've reached a bad 
                    // state due to pickList randomness. 
                    // Restart the selection for the current match.
                    if (!pickList.Any())
                    {
                        try 
                        {
                            return GenerateMatch(category, round, matchNumber);
                        }
                        catch
                        {
                            return -1;
                        }
                    }

                    var matchCounts = round.Matches
                        .SelectMany(p => p.MocIds)
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
                            .Where(m => !round.Matches.Last().MocIds.Contains(m));
                    match.MocIds.Add(pickList.Count()>0
                        ? pickList.First()
                        : -1);
                }
                round.Matches.Add(match);
                return matchNumber;
        }

        /// <summary>
        /// Find how many times id has faced other mocs from the current match
        /// </summary>
        /// <param name="round">The round</param>
        /// <param name="match">The match</param>
        /// <param name="id">id to check</param>
        /// <returns>Total</returns>
        private static int FacingCount(Round round, Match match, int id)
        {
            return round.Matches
                .Where(m => m.MocIds.Contains(id))
                .Where(m => match.MocIds.Any(i => m.MocIds.Contains(i)))
                .Count();
        }
    }
}
