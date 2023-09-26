using Microsoft.AspNetCore.SignalR;

namespace SignalRSample.Hubs
{
    public class NotificationHub : Hub
    {
        public static int NotificationCounter = 0;
        public static List<string> messages { get; set; } = new List<string>();


        public async Task SendMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                NotificationCounter++;
                messages.Add(message);
                await LoadMessages();
            }
        }

        public async Task LoadMessages()
        {
            await Clients.All.SendAsync("LoadNotifications", messages, NotificationCounter);

        }

    }
}
