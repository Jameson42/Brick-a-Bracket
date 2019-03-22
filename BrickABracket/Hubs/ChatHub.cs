using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BrickABracket.Core.ORM;

namespace BrickABracket.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub(Repository repo)
        {
            _repo = repo;
        }
        private Repository _repo;
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", _repo.ConnectionString);
        }
    }
}