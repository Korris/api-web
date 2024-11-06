using Microsoft.AspNetCore.SignalR;

namespace Mcsg.Lib.Common.Web.RealTime.Hubs;

using Mcsg.Common.Core.Requests;

public class CommonHub : Hub
{
    #region -- Overrides --

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("CommonHub-onConnected", $"ClientID: {Context.ConnectionId}");

        var hc = Context.GetHttpContext();
        if (hc == null)
        {
            return;
        }

        var req = new BaseR(hc);
        var userId = req.UserId;
        if (userId != null)
        {
            await JoinGroup(userId.Value.ToString());
        }
    }

    #endregion

    #region -- Methods --

    public CommonHub() { }

    private async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }

    #endregion
}
