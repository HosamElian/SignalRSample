using Microsoft.AspNetCore.SignalR;

namespace SignalRSample.Hubs
{
    public class UserHub : Hub
    {
        private readonly ILogger<string> _logger;

        public UserHub(ILogger<string> logger)
        {
            _logger = logger;
        }
        public static int TotalViews { get; set; } = 0;
        public static int TotalUsers { get; set; } = 0;

        public override Task OnConnectedAsync()
        {
            TotalUsers++;
            //_logger.LogInformation($"Number of user connected {TotalUsers}");
            Clients.All.SendAsync("updateTotalUsers", TotalUsers).GetAwaiter().GetResult();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            TotalUsers--;
            //_logger.LogInformation("1 user disconnected");
            //_logger.LogInformation($"Number of user connected {TotalUsers}");
            Clients.All.SendAsync("updateTotalUsers", TotalUsers).GetAwaiter().GetResult();

            return base.OnDisconnectedAsync(exception);
        }


        public async Task NewWindowLoaded()
        {
            TotalViews++;
            //_logger.LogInformation($"New of views {TotalUsers}");

            //send updates to all clienta that total views have been updated

            await Clients.All.SendAsync("updateTotalViews", TotalViews);
        }
    }
}
 