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
            int tournamentId):this(tournamentService, mocService, competitorService, 
            tournamentService.Read(tournamentId))
        {}
        public int GenerateRound(int categoryIndex, int roundIndex = -1)
        {
            //Ensure category exists
            var category = _tournament.Categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
                return -1;
            return GenerateRound(category, roundIndex);
        }
        private int GenerateRound(Category category, int roundIndex, bool removeOld = false)
        {
            //Ensure only one round exists
            //Derby tournaments only have one round with all matches and MOCs in it
            if (category.Rounds.Count>1)
                category.Rounds.RemoveRange(1, category.Rounds.Count-1);
            if (roundIndex == 0 && category.Rounds.Count == 1 && removeOld)
                category.Rounds[0] = new Round(){
                    MocIds = category.MocIds
                };
            if (category.Rounds.Count == 0)
                category.Rounds.Add(new Round(){
                    MocIds = category.MocIds
                });
            return 0;
        }
        public int GenerateMatch(int categoryIndex, int roundIndex = -1, int matchIndex = -1)
        {
            //Ensure category exists
            var category = _tournament.Categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
                return -1;

            //Ensure only one round exists
            //Derby tournaments only have one round with all matches in it
            if (category.Rounds.Count == 0)
                category.Rounds.Add(new Round());
            if (category.Rounds.Count>1)
                category.Rounds.RemoveRange(1, category.Rounds.Count-1);
            roundIndex = 0;
            var round = category.Rounds[roundIndex];
            return GenerateMatch(round, matchIndex);
        }
        private int GenerateMatch(Round round, int matchIndex, int retryAttempts = 10)
        {
            if (retryAttempts<=0)
                return -1;
            if (matchIndex<0)
                matchIndex = round.Matches.Count;
            if (matchIndex>=round.MocIds.Count && matchIndex>=MatchSize)
                return -1;
            if (round.Matches.Count > matchIndex)
            {
                round.Matches.RemoveAt(matchIndex);
                matchIndex = round.Matches.Count;
            }
            var match = new Match();
            for (int i=0;i<MatchSize;i++)
                {
                    var pickList = round.MocIds
                        .Where(p => !match.MocIds.Contains(p));

                    // If pickList is empty, we've reached a bad 
                    // state due to pickList randomness. 
                    // Restart the selection for the current match.
                    if (!pickList.Any())
                    {
                        try 
                        {
                            return GenerateMatch(round, matchIndex, retryAttempts - 1);
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
                    var fewestMatches = matchCounts.Any() ? matchCounts
                        .Select(m => m.Count)
                        .Min() : 0;
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
                return matchIndex;
        }
        public bool GenerateCategoryStandings(int categoryIndex)
        {
            var category = _tournament.Categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
                return false;
            return GenerateCategoryStandings(category);
        }
        private bool GenerateCategoryStandings(Category category)
        {
            foreach (var round in category.Rounds)
                GenerateRoundStandings(round);
            // Derby tournaments should only have 1 Round
            category.Standings = category.Rounds.FirstOrDefault()?.Standings;
            if (category.Standings == null)
                return false;
            return true;
        }
        public bool GenerateRoundStandings(int categoryIndex, int roundIndex)
        {
            var category = _tournament.Categories.ElementAtOrDefault(categoryIndex);
            if (category == null)
                return false;
            var round = category.Rounds.ElementAtOrDefault(roundIndex);
            if (round == null)
                return false;
            return GenerateRoundStandings(round);
        }
        private bool GenerateRoundStandings(Round round)
        {
            round.Standings = round.Matches
                .Where(m => m.Results.Any())
                .SelectMany(m => m.Results.LastOrDefault()?.Scores
                    ?.OrderBy(s => s.Time)
                    ?.Select((s, index) => new Standing(){
                        MocId = m.MocIds[s.Player-1],
                        Place = index + 1,
                        Score = 4 - index,
                        TotalTime = s.Time,
                        AverageTime = s.Time
                    })
                )
                .GroupBy(s => s.MocId)
                .OrderByDescending(g => g.Sum(s => s.Score))  //Higher = better
                .ThenBy(g => g.Sum(s => s.TotalTime))   //Total time breaks ties
                .Select((g, index) => new Standing(){
                    MocId = g.Key,
                    Score = g.Sum(s => s.Score),
                    TotalTime = g.Sum(s => s.TotalTime),
                    AverageTime = g.Sum(s => s.TotalTime)/Convert.ToDouble(g.Count()),
                    Place = index + 1
                }).ToList();
            return true;
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
