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
        private double MaxTime {get;} = 5.0;
        private const string patternPrefix = "Derby";
        public int GenerateRound(Category category, int roundIndex = -1)
        {
            if (category == null)
                return -1;
            //Ensure only one round exists
            //Derby tournaments only have one round with all matches and MOCs in it
            if (category.Rounds.Count>1)
                category.Rounds.RemoveRange(1, category.Rounds.Count-1);
            if (roundIndex == 0 && category.Rounds.Count == 1)
                category.Rounds[0] = new Round(){
                    MocIds = category.MocIds.ToList()
                };
            if (category.Rounds.Count == 0)
                category.Rounds.Add(new Round(){
                    MocIds = category.MocIds.ToList()
                });
            return 0;
        }
        public int GenerateMatch(Round round, int matchIndex = -1) => GenerateMatch(round, matchIndex, 25);
        private int GenerateMatch(Round round, int matchIndex, int retryAttempts)
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
            match.MocIds = new List<int>(new int[] {0, 0, 0, 0});
            if (!GenerateMatchLanes(round, match, 0))
                return -1;

            round.Matches.Add(match);
            return matchIndex;
        }

        private bool GenerateMatchLanes(Round round, Match match, int matchLane)
        {
            // If we successfully got past the final lane, the match is good
            if (matchLane >= MatchSize)
                return true;

            var pickList = round.MocIds
                .Where(p => !match.MocIds.Contains(p));

            // Remove MOCs that have already been in this lane
            pickList = pickList
                .Where(pl => !round.Matches.Any(m => m.MocIds.ElementAtOrDefault(matchLane) == pl));

            // If pickList is empty, we've reached a bad 
            // state due to pickList randomness. 
            if (!pickList.Any() && round.MocIds.Count>=MatchSize)
            {
                return false;
            }


            // How many times each MOC in the picklist is in existing Matches
            var matchCounts = pickList
                .GroupJoin(round.Matches
                    .SelectMany(p => p.MocIds)
                    .GroupBy(m => m), 
                    p => p, m => m.Key, 
                    (p,m) => new { Count = m?.Count() ?? 0, Moc = p }
                );
            
            // Lowest number of matches for each MOC
            var fewestMatches = matchCounts.Any() ? matchCounts
                .Select(m => m.Count)
                .Min() : 0;

            // MOCs with fewest matches
            var availableMocs = matchCounts
                .Where(m => m.Count == fewestMatches)
                .Select(m => m.Moc);

            // Reduce picklist
            pickList = availableMocs
                .OrderBy(p => FacingCount(round, match, p))
                .ThenBy(p => System.Guid.NewGuid()); // Random
            if (pickList?.Any(m => !round?.Matches?.LastOrDefault()?.MocIds?.Contains(m) ?? false) ?? false)
                pickList = pickList
                    .Where(m => !round.Matches.Last().MocIds.Contains(m));
            
            if (!pickList.Any())
            {
                match.MocIds[matchLane] = -1;
                return GenerateMatchLanes(round, match, matchLane + 1);
            }
            foreach(var moc in pickList.Union(availableMocs))
            {
                match.MocIds[matchLane] = moc;
                if (GenerateMatchLanes(round, match, matchLane + 1))
                    return true;
            }
            return false;
        }
        public bool GenerateCategoryStandings(Category category)
        {
            if (category == null)
                return false;
            foreach (var round in category.Rounds)
                GenerateRoundStandings(round);
            // Derby tournaments should only have 1 Round
            category.Standings = category.Rounds.FirstOrDefault()?.Standings;
            if (category.Standings == null)
                return false;
            return true;
        }
        public bool GenerateRoundStandings(Round round)
        {
            if (round == null)
                return false;
            EnsureMatchScores(round);
            round.Standings = round.Matches
                .Where(m => m.Results.Any())
                .SelectMany(m => m.Results.LastOrDefault()?.Scores
                    ?.OrderBy(s => s.Time)
                    ?.Select((s, index) => new Standing(){
                        MocId = m.MocIds[s.Player],
                        Place = index + 1,
                        Score = s.Time == MaxTime ? 0 : 4 - index,
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
                    TotalTime = g.Sum(s => Math.Round(s.TotalTime, 3)),
                    AverageTime = Math.Round(g.Sum(s => s.TotalTime)/Convert.ToDouble(g.Count()),3),
                    Place = index + 1
                }).ToList();
            return true;
        }
        private void EnsureMatchScores(Round round)
        {
            foreach (var match in round?.Matches)
            {
                foreach (var result in match?.Results)
                {
                    if (result.Scores == null)
                        continue;
                    for (int i = 0; i < MatchSize; i++)
                    {
                        if (!(result.Scores.Any(s => s.Player == i)))
                            result.Scores.Add(new Score(i, MaxTime));
                    }
                    foreach (var score in result.Scores)
                    {
                        if (score.Time > MaxTime)
                            score.Time = MaxTime;
                    }
                }
            }
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
