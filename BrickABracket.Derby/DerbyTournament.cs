using System;
using System.Collections.Generic;
using System.Linq;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Derby
{
    public class DerbyTournament: ITournamentStrategy
    {
        public int MatchSize {get;} = 4;
        public int GenerateRound(Category category, int roundIndex = -1, int runoff = 0)
        {
            if (category == null)
                return -1;
            var mocIds = category.MocIds.ToList();
            if (mocIds.Count == 3) // Avoid deadlock on exactly 3 MOCs
                mocIds.Add(-1);
            if (category.Rounds.Count == 0)
            {
                category.Rounds.Add(new Round(){
                    MocIds = mocIds
                });
                return 0;
            }
            if (roundIndex == 0 && category.Rounds.Count == 1)
            {
                category.Rounds[0] = new Round(){
                    MocIds = mocIds
                };
                return 0;
            }
            // Ensure first round has all mocs from category
            if (mocIds.Count > category.Rounds[0].MocIds.Count)
                category.Rounds[0].MocIds = mocIds;
            if (roundIndex == -1)
            {
                roundIndex = FirstUnfinishedRound(category);
                if (roundIndex > -1)
                    return roundIndex;
                roundIndex = category.Rounds.Count;
            }
            if (runoff > 0  && roundIndex <= category.Rounds.Count)
                mocIds = category.Rounds[roundIndex-1].Standings
                    .Select(s => s.MocId).Take(runoff).ToList();
            if (mocIds.Count == 3) // Avoid deadlock on exactly 3 MOCs
                mocIds.Add(-1);
            if (roundIndex < category.Rounds.Count)
                category.Rounds[roundIndex] = new Round(){
                    MocIds = mocIds
                };
            else if (roundIndex == category.Rounds.Count && runoff > 0)
                category.Rounds.Add(new Round(){
                    MocIds = mocIds
                });
            else return -1;
            return roundIndex;
        }
        private int FirstUnfinishedRound(Category category)
        {
            if (category == null)
                return -1;
            for (int i=0; i<category.Rounds.Count; i++)
            {
                var round = category.Rounds[i];
                if (round.Matches.Count < round.MocIds.Count ||
                    round.Matches.Count < MatchSize ||
                    round.Matches.Any(m => !m.Results.Any()))
                    return i;
            }
            return -1;
        }
        public int GenerateMatch(Round round, int matchIndex = -1)
        {
            if (round == null)
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
            var match = new Match
            {
                MocIds = new List<int>(new int[] { 0, 0, 0, 0 })
            };
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
            category.Standings = category.Rounds
                .SelectMany(r => r.Standings)
                .GroupBy(s => s.MocId)
                .OrderByDescending(g => g.Sum(s => s.Score))  //Higher = better
                .ThenBy(g => g.Sum(s => s.TotalTime))   //Total time breaks ties
                .Select((g, index) => new Standing(){
                    MocId = g.Key,
                    Place = index + 1,
                    Score = g.Sum(s => s.Score),
                    TotalTime = Math.Round(g.Sum(s => s.TotalTime),3),
                    AverageTime = Math.Round(g.Sum(s => s.TotalTime)/
                        Convert.ToDouble(g.Count(s => s.MocId == g.Key)),3)
                }).ToList();
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
                        Score = s.Time == 0.0 ? 0 : 4 - index,
                        TotalTime = s.Time,
                        AverageTime = s.Time
                    })
                )
                .Where(s => s.MocId != -1)  // Remove byes
                .GroupBy(s => s.MocId)
                .OrderByDescending(g => g.Sum(s => s.Score))  //Higher = better
                .ThenBy(g => g.Sum(s => s.TotalTime))   //Total time breaks ties
                .Select((g, index) => new Standing(){
                    MocId = g.Key,
                    Score = g.Sum(s => s.Score),
                    TotalTime = g.Sum(s => Math.Round(s.TotalTime, 3)),
                    AverageTime = Math.Round(g.Sum(s => s.TotalTime)/Convert.ToDouble(g.Where(s => s.TotalTime > 0.0).Count()),3),
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
                        // Empty scores = DNF (Did Not Finish)
                        if (!(result.Scores.Any(s => s.Player == i)))
                            result.Scores.Add(new Score(i, 0.0));
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
