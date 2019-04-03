using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BrickABracket.Core.Services;

namespace BrickABracket.Hubs
{
    public class TournamentHub : Hub
    {
        public TournamentHub(TournamentManager tm)
        {
            _tm = tm;
        }
        private TournamentManager _tm;
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", _tm);
        }
    }
}