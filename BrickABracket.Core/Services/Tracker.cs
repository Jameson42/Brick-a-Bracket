using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core
{
    public class Tracker: IScoreProvider, IStatusProvider
    {
        private Dictionary<IScoreProvider,IDisposable> _scoreSubscriptions;
        private Dictionary<IStatusProvider,IDisposable> _statusSubscriptions;
        private Subject<Score> _scores;
        private Subject<Status> _statuses;
        public IObservable<Score> Scores { get; private set; }
        public IObservable<Status> Statuses { get; private set; }

        // HashSet to ensure only one of each device object is kept
        private HashSet<IScoreProvider> _scoreProviders {get;}
        private HashSet<IStatusProvider> _statusProviders {get;}

        public Tracker()
        {
            _scores = new Subject<Score>();
            _statuses = new Subject<Status>();
            Scores = _scores.AsObservable();
            Statuses = _statuses.Replay(1);
            // Do I actually need to maintain these? Probably...
            _scoreProviders = new HashSet<IScoreProvider>();
            _statusProviders = new HashSet<IStatusProvider>();
            _scoreSubscriptions = new Dictionary<IScoreProvider, IDisposable>();
            _statusSubscriptions = new Dictionary<IStatusProvider, IDisposable>();
        }
        public bool AddScoreProvider(IScoreProvider device)
        {
            if (_scoreProviders.Contains(device) || _scoreSubscriptions.ContainsKey(device))
                return false;
            _scoreProviders.Add(device);
            _scoreSubscriptions.Add(device, device.Scores.Subscribe(PassScore));
            return true;
        }

        public void RemoveScoreProvider(IScoreProvider device)
        {
            if (_scoreProviders.Contains(device))
                _scoreProviders.Remove(device);
            if (_scoreSubscriptions.ContainsKey(device))
                _scoreSubscriptions[device].Dispose();
        }

        private void PassScore(Score score)
        {
            _scores.OnNext(score);
        }
    }
}