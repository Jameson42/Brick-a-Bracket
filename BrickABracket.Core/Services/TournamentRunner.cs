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
        private int _categoryIndex = -1;
        private int _roundIndex = -1;
        private int _matchIndex = -1;
        private Subject<Status> _statuses;
        private IDisposable _followStatusSubscription;
        private IDisposable _followScoreSubscription;
        private IEnumerable<Meta<Func<Tournament, ITournamentStrategy>>> _tournamentStrategies;
        #endregion
        public TournamentRunner(IEnumerable<Meta<Func<Tournament, ITournamentStrategy>>> tournamentStrategies)
        {
            _tournamentStrategies = tournamentStrategies;
            Statuses = _statuses.Replay(1);
        }
        public Tournament Tournament
        {
            get => _tournament;
            set
            {
                _tournament = value;
                CategoryIndex = -1;
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
                _categoryIndex = value;
                RoundIndex = -1;
            }
        }
        public int RoundIndex
        {
            get => _roundIndex;
            set
            {
                _roundIndex = value;
                MatchIndex = -1;
            }
        }
        public int MatchIndex
        {
            get => _matchIndex;
            set
            {
                _matchIndex = value;
            }
        }

        public IObservable<Status> Statuses {get;}

        public void FollowScores(IObservable<Score> scores)
        {
            if (_followScoreSubscription != null)
            {
                _followScoreSubscription.Dispose();
                _followScoreSubscription = null;
            }
            _followScoreSubscription = scores.Subscribe(ProcessScore);
        }
        public void FollowStatus(IObservable<Status> statuses)
        {
            if (_followStatusSubscription != null)
            {
                _followStatusSubscription.Dispose();
                _followStatusSubscription = null;
            }
            _followStatusSubscription = statuses.Subscribe(ProcessStatus);
        }

        /// <summary>
        /// Record score into current Match
        /// </summary>
        private void ProcessScore(Score score)
        {
            var match = _tournament
                ?.Categories?.ElementAtOrDefault(_categoryIndex)
                ?.Rounds?.ElementAtOrDefault(_roundIndex)
                ?.Matches?.ElementAtOrDefault(_matchIndex);
            if (match == null)
                return;
            if (score.player>_strategy.MatchSize || score.player<=0)
                return;
            // TODO: Record score on match
        }

        /// <summary>
        /// Act on incoming status
        /// </summary>
        private void ProcessStatus(Status status)
        {
            switch (status)
            {
                case Status.Start:
                case Status.Stop:
                    _statuses.OnNext(status);
                    break;
                case Status.Ready:
                case Status.Running:
                case Status.Stopped:
                case Status.Unknown:
                default:
                    // TODO: Log status?
                    break;
            }
        }

        public void Dispose()
        {
            if (_followScoreSubscription != null)
            {
                _followScoreSubscription.Dispose();
                _followScoreSubscription = null;
            }
            if (_followStatusSubscription != null)
            {
                _followStatusSubscription.Dispose();
                _followStatusSubscription = null;
            }
        }
    }
}