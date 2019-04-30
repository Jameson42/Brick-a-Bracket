using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Autofac.Features.Metadata;
using BrickABracket.Models.Interfaces;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    // Manage all aspects of running a tournament
    // - Change active portion of tournament
    // - Follow score/status providers
    // - Provide status to follower devices
    public class TournamentRunner: IStatusFollower, IStatusProvider, IDisposable
    {
        #region BackerVariables
        private Tournament _tournament;
        private ITournamentStrategy _strategy;
        private const int DEFAULTINDEX = -1;
        private int _categoryIndex = DEFAULTINDEX;
        private int _roundIndex = DEFAULTINDEX;
        private int _matchIndex = DEFAULTINDEX;
        private Subject<Status> _statuses = new Subject<Status>();
        private IDisposable _followStatusSubscription;
        private IDisposable _followScoreSubscription;
        private MocService _mocs;
        private IEnumerable<Meta<Func<Tournament, ITournamentStrategy>>> _tournamentStrategies;
        #endregion
        public TournamentRunner(MocService mocs, 
            IEnumerable<Meta<Func<Tournament, ITournamentStrategy>>> tournamentStrategies)
        {
            _mocs = mocs;
            _tournamentStrategies = tournamentStrategies;
            Statuses = _statuses.Replay(1);
        }
        public Tournament Tournament
        {
            get => _tournament;
            set
            {
                _tournament = value;
                CategoryIndex = DEFAULTINDEX;
                if (_tournament==null)
                    return;
                _strategy = _tournamentStrategies
                    .First(ts => (string)ts.Metadata["Type"] == _tournament.TournamentType)
                    .Value(_tournament);
            }
        }
        public int CategoryIndex
        {
            get => _categoryIndex;
            set
            {
                // TODO: Check validity
                _categoryIndex = value;
                RoundIndex = DEFAULTINDEX;
            }
        }
        public int RoundIndex
        {
            get => _roundIndex;
            set
            {
                // TODO: Check validity
                _roundIndex = value;
                MatchIndex = DEFAULTINDEX;
            }
        }
        public int MatchIndex
        {
            get => _matchIndex;
            set
            {
                // TODO: Check validity
                // Ask strategy for validity
                // Strategy can create match if necessary
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
        public IObservable<Status> Statuses {get;}
        public void FollowScores(IObservable<Score> scores)
        {
            UnFollowScores();
            _followScoreSubscription = scores.Subscribe(ProcessScore);
        }
        public void FollowStatus(IObservable<Status> statuses)
        {
            UnFollowStatus();
            _followStatusSubscription = statuses.Subscribe(ProcessStatus);
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
        public void GenerateCategories()
        {
            var mocs = _mocs.ReadAll()
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
        public bool NextMatch()
        {
            // Make next match active, create if necessary
            return false;
        }
        public void ReadyMatch()
        {
            if (MatchIsNull)
                return;
            Match.Results.Add(new MatchResult());
            _statuses.OnNext(Status.Ready);
        }
        public void StartMatch() => _statuses.OnNext(Status.Start);
        //TODO: Check if current match is valid first
        public void StopMatch() => _statuses.OnNext(Status.Stop);
        public void StartTimedMatch(long milliseconds)
        {
            StartMatch();
            Observable.Timer(TimeSpan.FromMilliseconds(milliseconds))
                .Subscribe(x => {
                    ProcessStatus(Status.Stop);
                });
        }

        //TODO: Generate round/category standings
        //TODO: Generate Match(es)

        /// <summary>
        /// Record score into current Match
        /// </summary>
        private void ProcessScore(Score score)
        {
            if (MatchIsNull)
                return;
            if (score.player>_strategy.MatchSize || score.player<=0)
                return;
            if (!Match.Results.Any())
                return; //Should be unreachable, Ready should always be sent before Start or scores
            Match.Results.Last().Scores.Add(score);
            _statuses.OnNext(Status.ScoreReceived);
        }
        private bool MatchIsNull => Match == null;

        /// <summary>
        /// Act on incoming status
        /// </summary>
        private void ProcessStatus(Status status)
        {
            switch (status)
            {
                case Status.Start:
                    StartMatch();
                    break;
                case Status.Stop:
                    StopMatch();
                    break;
                case Status.Ready:
                    // Only send Ready once in a row
                    if (_statuses.Latest().FirstOrDefault() != Status.Ready)
                        ReadyMatch();
                    break;
                case Status.Running:
                case Status.Stopped:
                    _statuses.OnNext(status);
                    break;
                case Status.Unknown:
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
            public Tournament Tournament;
            public int CategoryIndex;
            public int RoundIndex;
            public int MatchIndex;
        }
    }
}