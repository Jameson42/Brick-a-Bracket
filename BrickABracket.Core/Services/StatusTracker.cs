using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Core.Services
{
    public class StatusTracker : IStatusProvider
    {
        private readonly Dictionary<IStatusProvider, IDisposable> _statusSubscriptions;
        private readonly Subject<Status> _statuses;
        private HashSet<IStatusProvider> StatusProviders { get; }
        private readonly TournamentRunner _runner;
        public IObservable<Status> Statuses { get; private set; }

        public StatusTracker(TournamentRunner runner)
        {
            _statuses = new Subject<Status>();
            StatusProviders = new HashSet<IStatusProvider>();
            _statusSubscriptions = new Dictionary<IStatusProvider, IDisposable>();

            _runner = runner;
            // Runner needs to follow statuses from external status sources in order to start each match
            _runner.FollowStatus(_statuses.AsObservable());
            // Runner needs to provide statuses to external status readers 
            Statuses = _runner.Statuses;
        }
        public bool Add(IStatusProvider device)
        {
            if (StatusProviders.Contains(device) || _statusSubscriptions.ContainsKey(device))
                return false;
            StatusProviders.Add(device);
            _statusSubscriptions.Add(device, device.Statuses.Subscribe(PassStatus));
            return true;
        }
        public void Remove(IStatusProvider device)
        {
            if (StatusProviders.Contains(device))
                StatusProviders.Remove(device);
            if (_statusSubscriptions.ContainsKey(device))
            {
                _statusSubscriptions[device].Dispose();
                _statusSubscriptions.Remove(device);
            }
        }
        private void PassStatus(Status status)
        {
            _statuses.OnNext(status);
        }
    }
}