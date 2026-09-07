using Microsoft.AspNetCore.SignalR;

namespace Mcsg.Api.Areas.Realtime.Hubs;

using Common.Core.Requests;

public class NotificationHub : Hub
{
    #region -- Overrides --

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var req = new BaseR(Context.GetHttpContext());
        var userId = req.UserId;
        if (userId != null)
        {
            await JoinGroup(userId.Value.ToString());
        }

        await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public NotificationHub() { }

    private async Task JoinGroup(string group)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }

    #endregion
}
