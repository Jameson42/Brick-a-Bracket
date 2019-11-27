using Microsoft.AspNetCore.SignalR;
using System;
using System.Reactive.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Web.Hubs;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Hangfire;
using BrickABracket.Models.Base;

namespace BrickABracket.Web.Services
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            BackgroundJob.Enqueue(() => ExecuteAsync());
            // All IHostedService need to complete StartAsync so the next service can run
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync()
        {
            var statuses = _runner.Statuses
                .Do(async s => await ProcessStatus(s))
                .Select(_ => new object()); // Discard object, not needed for merge
            var scores = _scores.Scores
                .Do(async s => await ProcessScore(s))
                .Select(_ => new object()); // Discard object, not needed for merge
            // Send tournament update to clients after 0.5 seconds of no status activity
            _observable = statuses
                .Merge(scores)
                .TakeWhile(_ => !_cancellationToken.IsCancellationRequested)
                .Throttle(TimeSpan.FromMilliseconds(500));
            _watch = _observable.Subscribe(async _ => await UpdateTournamentViews());
            await _observable;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _watch?.Dispose();
            _watch = null;
            await _observable;
        }

        private async Task UpdateTournamentViews() =>
            await _hub.Clients.All.SendAsync("ReceiveTournament", _runner.Metadata);

        private async Task ProcessStatus(Status status) =>
            await _hub.Clients.All.SendAsync("ReceiveStatus", status);

        private async Task ProcessScore(Score score) =>
            await _hub.Clients.All.SendAsync("ReceiveScore", score);
    }
}