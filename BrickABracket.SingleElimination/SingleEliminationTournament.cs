using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Collections.Generic;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SingleElimination
{
    public class SingleEliminationTournament : ITournamentStrategy
    {
        public int MatchSize { get; } = 2;
        public int GenerateRound(Category category, int roundIndex = -1, int runoff = 0)
        {
            // TODO: If runoff, create a round to overcome top x tied scores
            if (category == null || roundIndex > category.Rounds.Count)
                return -1;
            if (category.Rounds.Count == 0)
            {
                category.Rounds.Add(new Round()
                {
                    MocIds = SeedSort(FillByes(category.MocIds))
                });
                return 0;
            }
            if (roundIndex == 0)
            {
                category.Rounds[0] = new Round()
                {
                    MocIds = SeedSort(FillByes(category.MocIds))
                };
                return 0;
            }
            if (roundIndex < 0)
            {
                roundIndex = FirstUnfinishedRound(category);
                if (roundIndex > -1) return roundIndex;
                if (category.Rounds.Last().Matches.Count == 1)
                {
                    if (runoff > 0) //Only allow runoff after 1st place achieved
                        return GenerateRunoffRound(category);
                    return -1;
                }
                roundIndex = category.Rounds.Count;
            }
            if (!GenerateRoundStandings(category.Rounds[roundIndex - 1]))
                return -1;
            var round = new Round()
            {
                MocIds = category.Rounds[roundIndex - 1]
                    .Standings.Where(s => s.Score > 0)
                    .Select(s => s.MocId).ToList()
            };
            if (category.Rounds.Count == roundIndex || roundIndex < 0)
            {
                category.Rounds.Add(round);
                return category.Rounds.Count - 1;
            }
            category.Rounds[roundIndex] = round;
            return roundIndex;
        }

        private int GenerateRunoffRound(Category category)
        {
            if (!GenerateCategoryStandings(category))
                return -1;
            var tiedScoreDifference = 4;
            var topTiedScore = category.Standings.First().Score + 1 - tiedScoreDifference;
            var tiedStandings = category.Standings.Where(s => s.Score == topTiedScore);
            while (tiedStandings.Count() < 2)
            {
                topTiedScore -= tiedScoreDifference;
                if (topTiedScore < 0)
                    return -1;
                tiedStandings = category.Standings.Where(s => s.Score == topTiedScore);
                tiedScoreDifference *= 2;
            }
            var round = new Round()
            {
                MocIds = tiedStandings.Select(s => s.MocId).ToList()
            };
            category.Rounds.Add(round);
            return category.Rounds.Count - 1;
        }

        private int FirstUnfinishedRound(Category category)
        {
            if (category == null)
                return -1;
            for (int i = 0; i < category.Rounds.Count; i++)
            {
                var round = category.Rounds[i];
                if (round.Matches.Count < (round.MocIds.Count / 2) ||
                    round.Matches.Any(m => !m.Results.Any()))
                    return i;
            }
            return -1;
        }

        // Return a "sorted" list based on standard bracketing logic. #1 vs #16, #2-#15, etc.
        private List<int> SeedSort(List<int> mocIds, int bundleSize = 1)
        {
            if (bundleSize * 4 > mocIds.Count)
                return mocIds;
            var cap = mocIds.Count;
            var list = new List<int>(cap);
            for (int i = 0; list.Count < cap; i += bundleSize)
            {
                list.AddRange(mocIds.GetRange(i, bundleSize));
                list.AddRange(mocIds.GetRange(cap - (i + bundleSize), bundleSize));
            }
            return SeedSort(list, bundleSize * 2);
        }

        private List<int> FillByes(List<int> mocIds)
        {
            var size = NextNearestPowerOfTwo(mocIds.Count);
            var list = mocIds.ToList();
            while (list.Count < size)
            {
                list.Add(-1);
            }
            return list;
        }

        private int NextNearestPowerOfTwo(int number)
        {
            if (number <= 0) return 0;
            --number;
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            return number + 1;
        }

        public int GenerateMatch(Round round, int matchIndex = -1)
        {
            if (round == null)
                return -1;
            if (matchIndex < 0)
                matchIndex = round.Matches.Count;
            if (matchIndex >= (round.MocIds.Count / 2))
            {
                GenerateRoundStandings(round);
                return -1;
            }
            var match = new Match
            {
                MocIds = round.MocIds.GetRange(matchIndex * 2, 2)
            };
            if (round.Matches.Count == matchIndex)
            {
                round.Matches.Add(match);
            }
            else round.Matches[matchIndex] = match;
            if (match.MocIds[1] == -1)  // SeedSort ensures bye is always second
            {
                // Auto-win against bye
                match.Results.Add(new MatchResult());
                match.Results[0].Scores.Add(new Score(0, 0));
                // Return next match index
                return GenerateMatch(round, matchIndex + 1);
            }
            return matchIndex;
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
                .OrderByDescending(g => g.Sum(s => s.Score))
                .Select((g, index) => new Standing()
                {
                    MocId = g.Key,
                    Place = index + 1,
                    Score = g.Sum(s => s.Score),
                    TotalTime = Math.Round(g.Sum(s => s.TotalTime), 3),
                    AverageTime = Math.Round(g.Sum(s => s.TotalTime) /
                        Convert.ToDouble(g.Count(s => s.MocId == g.Key && s.TotalTime > 0.0)), 3)
                }).ToList();
            // For any standings with equal scores, change Place to lowest place of those with that score
            for (int i = 1; i < category.Standings.Count; i++)
            {
                if (category.Standings[i].Score == category.Standings[i - 1].Score)
                    category.Standings[i].Place = category.Standings[i - 1].Place;
            }
            return true;
        }
        public bool GenerateRoundStandings(Round round)
        {
            if (round == null)
                return false;
            // Ensure 1st, 2nd place stay when doing runoffs of lower place values
            // i.e. in 8-moc, 1st has 7pts, 2nd has 6pts, 3rd/4th tie has 4pts and can only get 5 at most
            var roundScore = round.MocIds.Count / 2;
            // Assume first to score wins
            round.Standings = round.Matches
                .Where(m => m?.Results?.LastOrDefault()?.Scores?.FirstOrDefault() != null)
                .SelectMany(m => m.MocIds.Select(moc => new Standing()
                {
                    MocId = moc,
                    Score = m.MocIds[m.Results.Last().Scores.First().Player] == moc ? roundScore : 0,
                    TotalTime = m.Results.Last().Scores.First().Time,
                    AverageTime = m.Results.Last().Scores.First().Time
                })
                ).Where(s => s.MocId != -1)
                .ToList();
            return true;
        }
    }
}
