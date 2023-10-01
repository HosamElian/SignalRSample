using Microsoft.AspNetCore.SignalR;
using SignalRSample.Data;
using System.Security.Claims;

namespace SignalRSample.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var userName = _context.Users.FirstOrDefault(u => u.Id == userId).UserName;
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserConnected", userId, userName);
                HubConnections.AddUserConnection(userId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (HubConnections.HasUserConnection(userId, Context.ConnectionId))
            {
                var userConnections = HubConnections.Users[userId];
                userConnections.Remove(Context.ConnectionId); // need to explain
                HubConnections.Users.Remove(userId);
                if (userConnections.Count > 0)
                {
                    HubConnections.Users.Add(userId, userConnections);
                }
            }
            if (!string.IsNullOrEmpty(userId))
            {
                var userName = _context.Users.FirstOrDefault(u => u.Id == userId).UserName;
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserDisconnected", userId, userName);
                HubConnections.AddUserConnection(userId, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendAddRoomMessage(int maxRoom, int roomId, string roomName)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _context.Users.FirstOrDefault(u => u.Id == userId).UserName;
            await Clients.All.SendAsync("ReceiveAddRoomMessage", maxRoom, roomId, roomName, userId, userName);
        }

        public async Task SendDeleteRoomMessage(int deleted, int selected, string roomName)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _context.Users.FirstOrDefault(u => u.Id == userId).UserName;
            await Clients.All.SendAsync("ReceiveDeleteRoomMessage", deleted, selected, roomName, userId, userName);
        }

        public async Task SendPublicMessage(int roomId, string message, string roomName)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _context.Users.FirstOrDefault(u => u.Id == userId).UserName;
            await Clients.All.SendAsync("ReceivePublicMessage", roomId, message, roomName, userId, userName);
        }
        public async Task SendPrivateMessage(string receiverId, string message, string receiverName)
        {
            var senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderName = _context.Users.FirstOrDefault(u => u.Id == senderId).UserName;

            var users = new string[] { senderId, receiverId };
            
            await Clients.Users(users).SendAsync("ReceivePrivateMessage", senderId, senderName, receiverId, receiverName, message, Guid.NewGuid());
        }
        public async Task SendOpenPrivateChat(string receiverId)
        {
            var username = Context.User.FindFirstValue(ClaimTypes.Name);
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await Clients.User(receiverId).SendAsync("ReceiveOpenPrivateChat", userId, username);

        }
        public async Task SendDeletePrivateChat(string chatId)
        {

            await Clients.All.SendAsync("ReceiveDeletePrivateChat", chatId);

        }
        //public async Task SendMessageToAll(string sender, string message)
        //{
        //    await Clients.All.SendAsync("MessageReceived", sender, message);
        //}

        //[Authorize]
        //public async Task SendMessageToReceiver(string sender, string receiver, string message)
        //{
        //    var userId = _context.Users.FirstOrDefault(u => u.Email.ToLower() == receiver.ToLower()).Id;
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        await Clients.User(userId).SendAsync("MessageReceived", sender, message);
        //    }
        //}
    }
}
