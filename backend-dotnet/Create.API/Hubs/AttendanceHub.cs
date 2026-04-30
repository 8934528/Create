using Microsoft.AspNetCore.SignalR;

namespace Create.API.Hubs
{
    public class AttendanceHub : Hub
    {
        public async Task SendStatus(string message)
        {
            await Clients.All.SendAsync("ReceiveStatus", message);
        }
    }
}
