using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Services
{
    public class StatusPasser : IStatusProvider, IDisposable
    {
        private Subject<Status> _statuses;
        private StatusTracker _tracker;
        public StatusPasser(StatusTracker tracker)
        {
            _statuses = new Subject<Status>();
            Statuses = _statuses.AsObservable();
            tracker.Add(this);
            _tracker = tracker;
        }

        public IObservable<Status> Statuses { get; private set; }

        public void Dispose()
        {
            _tracker?.Remove(this);
            _statuses?.OnCompleted();
            _statuses?.Dispose();
        }

        public void PassStatus(Status status)
        {
            _statuses?.OnNext(status);
        }
    }
}