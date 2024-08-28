using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Hubs;

using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web.Security;
using Requests;

/// <summary>
/// Comment hub
/// </summary>
public class FollowHub : Hub
{
    private static readonly Dictionary<Guid, string> _userConnections = new Dictionary<Guid, string>();
    private readonly ICurrentUserService _currentUserService;
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="followService"></param>
    public FollowHub(IFollowService followService, ICurrentUserService currentUserService,
        IFollowPostService followPostService)
    {
        _followService = followService;
        _currentUserService = currentUserService;
        _followPostService = followPostService;
    }

    /// <summary>
    /// OnConnected async
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var user = await _currentUserService.GetCurrentUserAsync(); // Get user ID (make sure it's configured)
        var connectionId = Context.ConnectionId;

        if (user != null)
        {
            _userConnections[user.UserId.Value] = connectionId;
        }
        await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
    }

    /// <summary>
    /// follow user
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    //[Authorize]
    public async Task FollowUser(FollowUserR request)
    {
        var resp = await _followService.FollowUser(request);

        var clientId = _userConnections.FirstOrDefault(p => p.Key == request.CreatedByUserId).Value;
        await Clients.Client(clientId).SendAsync(RealTimeTopic.ReceiveFollow, JsonConvert.SerializeObject(resp));
    }

    public async Task FollowPost(FollowPostReq request)
    {
        await _followPostService.SendPostFollowNotification(request);
    }

    public async Task OnDisconnectedAsync()
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        var userId = user.UserId.Value;

        if (user != null && _userConnections.ContainsKey(userId))
        {
            _userConnections.Remove(userId);
        }
    }
    #endregion

    #region -- Fields --

    /// <summary>
    /// Follow Service
    /// </summary>
    private readonly IFollowService _followService;

    private readonly IFollowPostService _followPostService;

    #endregion
}
