using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using BrickABracket.NXT;

namespace BrickABracket.Core
{
    public class Tracker
    {
        private Dictionary<IDevice,IDisposable> _scoreSubscriptions;
        private Dictionary<IDevice,IDisposable> _statusSubscriptions;
        private Subject<IScore> _scores;
        private Subject<Status> _statuses;
        public IObservable<IScore> Scores { get; private set; }
        public IObservable<Status> Statuses { get; private set; }

        // HashSet to ensure only one of each device object is kept
        private HashSet<IDevice> _connectedDevices {get;}
        private HashSet<IScoreProvider> _scoreProviders {get;}
        private HashSet<IStatusProvider> _statusProviders {get;}

        public Tracker()
        {
            _scores = new Subject<IScore>();
            _statuses = new Subject<Status>();
            Scores = _scores.AsObservable();
            Statuses = _statuses.Replay(1);
            // Do I actually need to maintain these? Probably...
            _connectedDevices = new HashSet<IDevice>();
            _scoreProviders = new HashSet<IScoreProvider>();
            _statusProviders = new HashSet<IStatusProvider>();
            _scoreSubscriptions = new Dictionary<IDevice, IDisposable>();
            _statusSubscriptions = new Dictionary<IDevice, IDisposable>();
        }

        // TODO: Methods to add devices w/ types (score/status/etc)
        // Methods to remove devices
        public bool AddScoreProvider(IDevice device)
        {
            if (_scoreProviders.Contains(device) || _scoreSubscriptions.ContainsKey(device))
                return false;
            _scoreProviders.Add(device);
            _scoreSubscriptions.Add(device, device.Scores.Subscribe(PassScore));
            return true;
        }

        public void RemoveNxtScoreProvider(string connectionString)
        {
            RemoveScoreProvider(new Nxt(connectionString));
        }

        public void RemoveScoreProvider(IDevice device)
        {
            if (_scoreProviders.Contains(device) || _scoreSubscriptions.ContainsKey(device))
                return;
            _scoreProviders.Remove(device);
            _scoreSubscriptions[device].Dispose();
        }

        private void PassScore(IScore score)
        {
            _scores.OnNext(score);
        }
    }
}