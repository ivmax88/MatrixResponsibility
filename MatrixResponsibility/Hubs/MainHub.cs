using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MatrixResponsibility.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        public MainHub()
        {
            
        }
        public async Task Send(string message)
        {
            var user = Context.User;
            await Clients.All.SendAsync("all", message);
        }
    }
}
