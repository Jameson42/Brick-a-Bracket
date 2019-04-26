using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core.Services
{
    public class ScoreTracker: IScoreProvider
    {
        private Dictionary<IScoreProvider,IDisposable> _scoreSubscriptions;
        private Subject<Score> _scores;
        private HashSet<IScoreProvider> _scoreProviders {get;}
        private TournamentRunner _runner;
        public IObservable<Score> Scores { get; private set; }

        public ScoreTracker(TournamentRunner runner)
        {
            _scores = new Subject<Score>();
            _scoreProviders = new HashSet<IScoreProvider>();
            _scoreSubscriptions = new Dictionary<IScoreProvider, IDisposable>();

            _runner = runner;
            // Runner needs to follow scores from external score sources to update the active tournament
            _runner.FollowScores(Scores);
            Scores = _scores.AsObservable();
        }
        public bool Add(IScoreProvider device)
        {
            if (_scoreProviders.Contains(device) || _scoreSubscriptions.ContainsKey(device))
                return false;
            _scoreProviders.Add(device);
            _scoreSubscriptions.Add(device, device.Scores.Subscribe(PassScore));
            return true;
        }
        public void Remove(IScoreProvider device)
        {
            if (_scoreProviders.Contains(device))
                _scoreProviders.Remove(device);
            if (_scoreSubscriptions.ContainsKey(device))
            {
                _scoreSubscriptions[device].Dispose();
                _scoreSubscriptions.Remove(device);
            }
        }
        private void PassScore(Score score)
        {
            _scores.OnNext(score);
        }
    }
}