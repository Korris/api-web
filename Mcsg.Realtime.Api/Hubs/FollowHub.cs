using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Hubs;

using Common.Core.Requests;
using Interfaces;
using Lib.Common.Constants;
using Requests;

/// <summary>
/// Comment hub
/// </summary>
public class FollowHub : Hub
{
    #region -- Overrides --

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous connect.</returns>
    public override async Task OnConnectedAsync()
    {
        var req = new BaseR(Context.GetHttpContext());
        var userId = req.UserId;
        if (userId != null)
        {
            _userConnections[userId.Value] = Context.ConnectionId;
        }

        await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous disconnect.</returns>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var req = new BaseR(Context.GetHttpContext());
        var userId = req.UserId ?? Guid.Empty;

        if (_userConnections.ContainsKey(userId))
        {
            _userConnections.Remove(userId);
        }

        return base.OnDisconnectedAsync(exception);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="followService"></param>
    /// <param name="followPostService"></param>
    public FollowHub(IFollowService followService, IFollowPostService followPostService)
    {
        _followService = followService;
        _followPostService = followPostService;
    }

    /// <summary>
    /// FollowUser
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    public async Task FollowUser(FollowUserR request)
    {
        request.Analyze(Context.GetHttpContext());
        var resp = await _followService.FollowUser(request);

        var clientId = _userConnections.FirstOrDefault(p => p.Key == request.CreatedByUserId).Value;
        await Clients.Client(clientId).SendAsync(RealTimeTopic.ReceiveFollow, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// FollowPost
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    public async Task FollowPost(FollowPostReq request)
    {
        request.Analyze(Context.GetHttpContext());
        await _followPostService.SendPostFollowNotification(request);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Follow service
    /// </summary>
    private readonly IFollowService _followService;

    private readonly IFollowPostService _followPostService;

    private static readonly Dictionary<Guid, string> _userConnections = [];

    #endregion
}
