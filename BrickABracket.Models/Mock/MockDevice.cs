using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Mock
{
    public class MockDevice : IDevice
    {
        private readonly Subject<Score> _scores;
        private readonly BehaviorSubject<Status> _statuses;
        private IDisposable _followSubscription;
        public IObservable<Score> Scores { get; private set; }
        public IObservable<Status> Statuses { get; private set; }
        private readonly Func<string, Score> _scoreFactory;
        public MockDevice(string connectionString, Func<string, Score> scoreFactory)
        {
            _scoreFactory = scoreFactory;
            _scores = new Subject<Score>();
            _statuses = new BehaviorSubject<Status>(Status.Ready);
            Scores = _scores.AsObservable();
            Statuses = _statuses.AsObservable();
        }
        public bool Connected => true;

        public string Connection => "mock";

        public ushort BatteryLevel => 100;

        public string BrickName => "Mock Device";

        public string Program { get; set; }
        public IEnumerable<string> Programs => GetPrograms();

        public IEnumerable<string> GetPrograms()
        {
            return new string[] { "Mock Program", "Another Program" };
        }

        public bool Connect() => true;

        public void Dispose()
        {
            UnFollowStatus();
            _scores?.OnCompleted();
            _scores?.Dispose();
            _statuses?.OnCompleted();
            _statuses?.Dispose();
        }

        public void FollowStatus(IObservable<Status> statuses)
        {
            UnFollowStatus();
            _followSubscription = statuses.Subscribe(s =>
            {
                switch (s)
                {
                    case Status.Start:
                        var random = new Random();
                        for (int i = 0; i < 4; i++)
                            _scores.OnNext(new Score(i, Math.Round(random.NextDouble() * 5.0, 3)));
                        _statuses.OnNext(Status.Stop);
                        break;
                    case Status.Ready:
                        _statuses.OnNext(Status.Ready);
                        break;
                    default:
                        break;
                }
            });
        }
        public void UnFollowStatus()
        {
            if (_followSubscription != null)
            {
                _followSubscription.Dispose();
                _followSubscription = null;
            }
        }
    }
}