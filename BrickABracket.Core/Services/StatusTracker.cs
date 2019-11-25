using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BrickABracket.Core.Services
{
    public class StatusTracker : IStatusProvider, IDisposable
    {
        private readonly Dictionary<IStatusProvider, IDisposable> _statusSubscriptions;
        private readonly Subject<Status> _statuses;
        private HashSet<IStatusProvider> StatusProviders { get; }
        private readonly TournamentRunner _runner;
        public IObservable<Status> Statuses { get; private set; }
        private readonly ILogger<StatusTracker> _logger;
        private readonly IDisposable _runnerSubscription;

        public StatusTracker(TournamentRunner runner, ILogger<StatusTracker> logger)
        {
            _runner = runner;
            _logger = logger;

            _statuses = new Subject<Status>();
            StatusProviders = new HashSet<IStatusProvider>();
            _statusSubscriptions = new Dictionary<IStatusProvider, IDisposable>();
            // Runner needs to follow statuses from external status sources in order to start each match
            _runner.FollowStatus(_statuses.AsObservable());
            // Runner needs to provide statuses to external status readers 
            Statuses = _runner.Statuses;
            _runnerSubscription = Statuses.Subscribe(LogStatus);
        }
        public bool Add(IStatusProvider device)
        {
            if (StatusProviders.Contains(device) || _statusSubscriptions.ContainsKey(device))
                return false;
            StatusProviders.Add(device);
            // Don't pass Ready status to runner, it can cause a loop
            _statusSubscriptions.Add(device,
                device.Statuses.Where(s => s.Code != StatusCode.Ready).Subscribe(PassStatus)
            );
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
        public void PassStatus(Status status)
        {
            LogStatus(status);
            _statuses.OnNext(status);
        }
        private void LogStatus(Status status) =>
            _logger.LogDebug("New status: {Status}", status);

        public void Dispose()
        {
            _runnerSubscription?.Dispose();
            _statuses?.OnCompleted();
            _statuses?.Dispose();
        }
    }
}