using Microsoft.AspNetCore.SignalR;

namespace Mcsg.Lib.Common.Web.RealTime.Hubs;

using Security;

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
