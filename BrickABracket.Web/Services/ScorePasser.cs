using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Services
{
    public class ScorePasser : IScoreProvider, IDisposable
    {
        private readonly Subject<Score> _scores;
        private readonly ScoreTracker _tracker;
        public ScorePasser(ScoreTracker tracker)
        {
            _scores = new Subject<Score>();
            Scores = _scores.AsObservable();
            tracker.Add(this);
            _tracker = tracker;
        }

        public IObservable<Score> Scores { get; private set; }

        public void Dispose()
        {
            _tracker?.Remove(this);
            _scores?.OnCompleted();
            _scores?.Dispose();
        }

        public void PassScore(Score score)
        {
            _scores?.OnNext(score);
        }
    }
}