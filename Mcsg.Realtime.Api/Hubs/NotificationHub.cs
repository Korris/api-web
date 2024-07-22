using Microsoft.AspNetCore.SignalR;

namespace Mcsg.Realtime.Api.Hubs
{
    using Lib.Common.Web.Security;

    public class NotificationHub : Hub
    {
        private readonly ICurrentUserService _currentUserService;
        public NotificationHub(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");

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
