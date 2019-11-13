using Microsoft.AspNetCore.SignalR;
using System;
using System.Reactive.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Hubs;

namespace BrickABracket.Services
{
    public class MatchWatcher : IDisposable
    {
        private readonly TournamentRunner _runner;
        private readonly ScoreTracker _scores;
        private readonly IHubContext<TournamentHub> _hub;
        private IDisposable _watch;
        public MatchWatcher(TournamentRunner runner,
            ScoreTracker scores,
            IHubContext<TournamentHub> hub)
        {
            _runner = runner;
            _scores = scores;
            _hub = hub;
            // Send tournament update to clients after 0.5 seconds of no status activity
            _watch = _runner.Statuses
                .Select(s => new object())  // Don't need the data, this is so merge works
                .Merge(_scores.Scores)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(x =>
                {
                    _hub.Clients.All
                        .SendAsync("ReceiveTournament", _runner.Metadata);
                });
            // TODO: Also send individual status to frontend
        }

        public void Dispose()
        {
            _watch?.Dispose();
            _watch = null;
        }
    }
}