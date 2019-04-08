using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BrickABracket.Core.Services;

namespace BrickABracket.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(ActivesService repo)
        {
            _repo = repo;
        }
        private ActivesService _repo;
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", _repo.ToString());
        }
    }
}