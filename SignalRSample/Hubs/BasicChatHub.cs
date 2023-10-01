using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSample.Data;

namespace SignalRSample.Hubs
{
    public class BasicChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public BasicChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessageToAll(string sender, string message)
        {
            await Clients.All.SendAsync("MessageReceived", sender, message);
        }

        [Authorize]
        public async Task SendMessageToReceiver(string sender, string receiver, string message)
        {
            var userId = _context.Users.FirstOrDefault(u => u.Email.ToLower() == receiver.ToLower()).Id;
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.User(userId).SendAsync("MessageReceived", sender, message);
            }
        }
    }
}
