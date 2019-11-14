using Microsoft.AspNetCore.SignalR;
using System;
using System.Reactive.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Hubs;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace BrickABracket.Services
{
    public class MatchWatcher : IHostedService, IDisposable
    {
        private readonly TournamentRunner _runner;
        private readonly ScoreTracker _scores;
        private readonly IHubContext<TournamentHub> _hub;
        private IObservable<object> _observable;
        private IDisposable _watch;
        private CancellationToken _cancellationToken;
        public MatchWatcher(TournamentRunner runner,
            ScoreTracker scores,
            IHubContext<TournamentHub> hub)
        {
            _runner = runner;
            _scores = scores;
            _hub = hub;
            // TODO: Also send individual status to frontend?
        }

        public void Dispose()
        {
            _watch?.Dispose();
            _watch = null;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            // Send tournament update to clients after 0.5 seconds of no status activity
            _observable = _runner.Statuses
                .Select(s => new object())  // Don't need the data, this is so merge works
                .Merge(_scores.Scores)
                .TakeWhile(_ => !_cancellationToken.IsCancellationRequested)
                .Throttle(TimeSpan.FromMilliseconds(500));
            _watch = _observable.Subscribe(x =>
            {
                _hub.Clients.All
                    .SendAsync("ReceiveTournament", _runner.Metadata);
            });
            await _observable;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _watch?.Dispose();
            _watch = null;
            await _observable;
        }
    }
}