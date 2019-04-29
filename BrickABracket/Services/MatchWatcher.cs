using Microsoft.AspNetCore.SignalR;
using System;
using System.Reactive.Linq;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;
using BrickABracket.Hubs;

namespace BrickABracket.Services
{
    public class MatchWatcher: IDisposable
    {
        private readonly TournamentRunner _runner;
        private readonly IHubContext<TournamentHub> _hub;
        private IDisposable _watch;
        public MatchWatcher(TournamentRunner runner,
            IHubContext<TournamentHub> hub)
        {
            _runner = runner;
            _hub = hub;
            // Send tournament update to clients after 0.5 seconds of no status activity
            _watch = _runner.Statuses
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(x => {
                    _hub.Clients.All
                        .SendAsync("ReceiveTournament", _runner.Metadata);
                });
            // TODO: Also send individual status to frontend
        }

        public void Dispose()
        {
            if (_watch != null)
                _watch.Dispose();
            _watch = null;
        }
    }
}