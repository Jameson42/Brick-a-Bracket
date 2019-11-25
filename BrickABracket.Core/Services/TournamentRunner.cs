using Autofac.Features.Metadata;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace BrickABracket.Core.Services
{
    // Manage all aspects of running a tournament
    // - Change active portion of tournament
    // - Follow score/status providers
    // - Provide status to follower devices
    public class TournamentRunner : IStatusFollower, IStatusProvider, IDisposable
    {
        #region BackerVariables
        private Tournament _tournament;
        private ITournamentStrategy _strategy;
        private const int DEFAULTINDEX = -1;
        private int _categoryIndex = DEFAULTINDEX;
        private int _roundIndex = DEFAULTINDEX;
        private int _matchIndex = DEFAULTINDEX;
        private readonly BehaviorSubject<Status> _statuses =
            new BehaviorSubject<Status>(Status(StatusCode.Unknown));
        private IDisposable _followStatusSubscription;
        private IDisposable _followScoreSubscription;
        private readonly Func<Repository<Moc>> _mocRepositoryFactory;
        private readonly Func<Repository<Tournament>> _tournamentRepositoryFactory;
        private readonly IEnumerable<Meta<ITournamentStrategy>> _tournamentStrategies;
        private readonly ILogger<TournamentRunner> _logger;
        #endregion
        public TournamentRunner(Func<Repository<Moc>> mocs, Func<Repository<Tournament>> tournaments,
            IEnumerable<Meta<ITournamentStrategy>> tournamentStrategies,
            ILogger<TournamentRunner> logger)
        {
            _mocRepositoryFactory = mocs;
            _tournamentRepositoryFactory = tournaments;
            _tournamentStrategies = tournamentStrategies;
            _logger = logger;
            Statuses = _statuses.AsObservable();
        }
        public Tournament Tournament
        {
            get => _tournament;
            set
            {
                if (value == null)
                {
                    _tournament = null;
                    CategoryIndex = DEFAULTINDEX;
                    return;
                }
                if (_tournament?.TournamentType != value.TournamentType)
                    _strategy = _tournamentStrategies
                        .FirstOrDefault(ts => (string)ts.Metadata["Type"] == value.TournamentType)
                        ?.Value;
                if (_tournament != value)
                    CategoryIndex = DEFAULTINDEX;
                _tournament = value;
            }
        }
        public int CategoryIndex
        {
            get => _categoryIndex;
            set
            {
                if (_categoryIndex == value)
                    return;
                if (value < -1 || value >= (Tournament?.Categories?.Count ?? 0))
                    return;
                _categoryIndex = value;
                RoundIndex = DEFAULTINDEX;
            }
        }
        public int RoundIndex
        {
            get => _roundIndex;
            set
            {
                if (_roundIndex == value)
                    return;
                if (value < -1 || value >= (Category?.Rounds?.Count ?? 0))
                    return;
                _roundIndex = value;
                MatchIndex = DEFAULTINDEX;
            }
        }
        public int MatchIndex
        {
            get => _matchIndex;
            set
            {
                if (_matchIndex == value)
                    return;
                if (value < -1 || value >= (Round?.Matches?.Count ?? 0))
                    return;
                _matchIndex = value;
            }
        }
        public Category Category => Tournament?.Categories?.ElementAtOrDefault(CategoryIndex);
        public Round Round => Category?.Rounds?.ElementAtOrDefault(RoundIndex);
        public Match Match => Round?.Matches?.ElementAtOrDefault(MatchIndex);
        public TournamentMetadata Metadata
        {
            get
            {
                return new TournamentMetadata()
                {
                    Tournament = this.Tournament,
                    CategoryIndex = this.CategoryIndex,
                    RoundIndex = this.RoundIndex,
                    MatchIndex = this.MatchIndex
                };
            }
        }
        private async Task<bool> SaveTournament()
        {
            if (Tournament != null && Tournament._id > 0)
                using (var _tournaments = _tournamentRepositoryFactory())
                    return (await _tournaments.CreateOrUpdateAsync(Tournament)) > 0;
            return false;
        }
        private static Status Status(StatusCode code) =>
            new Status(code, typeof(TournamentRunner), "Tournament Runner");
        public IObservable<Status> Statuses { get; }
        public void FollowScores(IObservable<Score> scores)
        {
            UnFollowScores();
            if (scores == null)
                return;
            _followScoreSubscription = scores
                .Subscribe(async s => await ProcessScore(s));
        }
        public void FollowStatus(IObservable<Status> statuses)
        {
            UnFollowStatus();
            if (statuses == null)
                return;
            _followStatusSubscription = statuses
                .Subscribe(async s => await ProcessStatus(s));
        }
        public void UnFollowScores()
        {
            if (_followScoreSubscription != null)
            {
                _followScoreSubscription.Dispose();
                _followScoreSubscription = null;
            }
        }
        public void UnFollowStatus()
        {
            if (_followStatusSubscription != null)
            {
                _followStatusSubscription.Dispose();
                _followStatusSubscription = null;
            }
        }
        //Create/Update categories for current Tournament
        public async Task GenerateCategories()
        {
            IEnumerable<Moc> mocs;
            using (var _mocs = _mocRepositoryFactory())
            {
                mocs = _mocs.ReadAll()
                    .Where(m => Tournament.MocIds.Contains(m._id));
                var classifications = mocs
                    .Select(m => m.ClassificationId)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                // Add Categories
                foreach (var classification in classifications)
                {
                    if (Tournament.Categories.Any(c => c.ClassificationId == classification))
                        continue;
                    Tournament.Categories.Add(new Category(classification));
                }
                // Put MOCs in categories
                foreach (var category in Tournament.Categories)
                {
                    category.MocIds = mocs
                        .Where(m => m.ClassificationId == category.ClassificationId)
                        .Select(m => m._id)
                        .ToList();
                    // Remove Categories w/ no MOCs in Tournament (RARE)
                    if (!category.MocIds.Any())
                        Tournament.Categories.Remove(category);
                }
            }

            await SaveTournament();
        }
        public async Task<bool> Runoff(int count)
        {
            if (Category == null)
                return false;
            RoundIndex = _strategy.GenerateRound(Category, -1, count);
            await SaveTournament();
            return RoundIndex != -1;
        }
        public async Task<bool> NextMatch()
        {
            // Create next match in category and activate
            if (Category == null)
                return false;
            // Select first unfinished round
            RoundIndex = _strategy.GenerateRound(Category);
            // Select next match
            MatchIndex = Round?.Matches?.FindIndex(m => m?.Results?.Count() == 0) ?? -1;
            if (MatchIndex == -1)
                MatchIndex = _strategy.GenerateMatch(Round);
            await SaveTournament();
            return MatchIndex > -1;
        }
        public async Task ReadyMatch()
        {
            if (MatchIsNull)
                return;
            var lastResult = Match.Results.LastOrDefault();
            // Only add new result if last result has scores
            if (lastResult == null || lastResult.Scores.Any())
                Match.Results.Add(new MatchResult());
            _statuses.OnNext(Status(StatusCode.Ready));
            await SaveTournament();
        }
        public bool StartMatch()
        {
            if (MatchIsNull)
                return false;
            _statuses.OnNext(Status(StatusCode.Stop));
            return true;
        }
        public async Task StopMatch()
        {
            _statuses.OnNext(Status(StatusCode.Stop));
            _strategy.GenerateCategoryStandings(Category);
            await SaveTournament();
        }
        public bool StartTimedMatch(long milliseconds)
        {
            if (!StartMatch())
                return false;
            Observable.Timer(TimeSpan.FromMilliseconds(milliseconds))
                .Subscribe(async x => await ProcessStatus(Status(StatusCode.Stop)));
            return true;
        }
        public async Task<bool> DeleteCurrentMatch()
        {
            if (Match == null)
                return false;
            Round.Matches.RemoveAt(MatchIndex);
            MatchIndex = DEFAULTINDEX;
            await SaveTournament();
            return true;
        }
        public bool GenerateRoundStandings(int categoryIndex = -1, int roundIndex = -1)
        {
            if (categoryIndex == -1 || roundIndex == -1)
                return _strategy.GenerateRoundStandings(this.Round);
            var round = Tournament?.Categories
                ?.ElementAtOrDefault(categoryIndex)
                ?.Rounds?.ElementAtOrDefault(roundIndex);
            if (round == null)
                return false;
            return _strategy.GenerateRoundStandings(round);
        }
        public bool GenerateCategoryStandings(int categoryIndex = -1)
        {
            if (categoryIndex == -1)
                return _strategy.GenerateCategoryStandings(this.Category);
            var category = Tournament?.Categories?.ElementAtOrDefault(categoryIndex);
            if (category == null)
                return false;
            return _strategy.GenerateCategoryStandings(category);
        }
        public async Task<bool> GenerateAllStandings()
        {
            foreach (var category in Tournament?.Categories)
                _strategy.GenerateCategoryStandings(category);
            await SaveTournament();
            return true;
        }
        /// <summary>
        /// Record score into current Match
        /// </summary>
        private async Task ProcessScore(Score score)
        {
            if (MatchIsNull)
                return;
            if (score.Player > _strategy.MatchSize || score.Player < 0)
                return;
            if (!Match.Results.Any())
                return; //Should be unreachable, Ready should always be sent before Start or scores
            Match.Results.Last().Scores.Add(score);
            _statuses.OnNext(Status(StatusCode.ScoreReceived));
            await SaveTournament();
        }
        private bool MatchIsNull => Match == null;

        /// <summary>
        /// Act on incoming status
        /// </summary>
        private async Task ProcessStatus(Status status)
        {
            switch (status.Code)
            {
                case StatusCode.Start:
                    StartMatch();
                    break;
                case StatusCode.Stop:
                    await StopMatch();
                    break;
                case StatusCode.Ready:
                    await ReadyMatch();
                    break;
                case StatusCode.Running:
                case StatusCode.Stopped:
                case StatusCode.Unknown:
                default:
                    // TODO: Log status?
                    break;
            }
        }

        public void Dispose()
        {
            UnFollowScores();
            UnFollowStatus();
            _statuses.OnCompleted();
        }

        public class TournamentMetadata
        {
            public Tournament Tournament { get; set; }
            public int CategoryIndex { get; set; }
            public int RoundIndex { get; set; }
            public int MatchIndex { get; set; }
        }
    }
}