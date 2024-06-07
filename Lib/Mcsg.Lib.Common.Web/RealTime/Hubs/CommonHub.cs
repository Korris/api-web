using Mcsg.Lib.Common.Web.Security;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Mcsg.Lib.Common.Web.RealTime.Hubs
{
    public class CommonHub : Hub
    {
        private readonly ICurrentUserService _currentUserService;
        public CommonHub(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("CommonHub-onConnected", $"ClientID: {Context.ConnectionId}");

            var user = await _currentUserService.GetCurrentUserAsync();
            if (user != null)
            {
                await JoinGroup(user.UserId.ToString());
            }
        }
        private async Task JoinGroup(string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }
}
