using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BrickABracket.Core.Services
{
    public class ScoreTracker : IScoreProvider
    {
        private readonly Dictionary<IScoreProvider, IDisposable> _scoreSubscriptions;
        private readonly Subject<Score> _scores;
        private HashSet<IScoreProvider> ScoreProviders { get; }
        private readonly TournamentRunner _runner;
        public IObservable<Score> Scores { get; private set; }

        public ScoreTracker(TournamentRunner runner)
        {
            _scores = new Subject<Score>();
            ScoreProviders = new HashSet<IScoreProvider>();
            _scoreSubscriptions = new Dictionary<IScoreProvider, IDisposable>();

            _runner = runner;
            // Runner needs to follow scores from external score sources to update the active tournament
            Scores = _scores.AsObservable();
            _runner.FollowScores(Scores);
        }
        public bool Add(IScoreProvider device)
        {
            if (ScoreProviders.Contains(device) || _scoreSubscriptions.ContainsKey(device))
                return false;
            ScoreProviders.Add(device);
            _scoreSubscriptions.Add(device, device.Scores.Subscribe(PassScore));
            return true;
        }
        public void Remove(IScoreProvider device)
        {
            if (ScoreProviders.Contains(device))
                ScoreProviders.Remove(device);
            if (_scoreSubscriptions.ContainsKey(device))
            {
                _scoreSubscriptions[device].Dispose();
                _scoreSubscriptions.Remove(device);
            }
        }
        public void PassScore(Score score) => _scores.OnNext(score);
    }
}