using Microsoft.AspNetCore.SignalR;

namespace Mcsg.Lib.Common.Web.RealTime.Services;

using Hubs;

public interface ISignalRService
{
    Task SendToUser<T>(string topic, string userId, T message);
    Task SendToGroup<T>(string topic, string groupId, T message);
    Task SendToAll<T>(string topic, T message);
    Task SendToUser(string topic, string userId, string message);
    Task SendToGroup(string topic, string groupId, string message);
    Task SendToAll(string topic, string message);
}
public class SignalRService : ISignalRService
{
    private readonly IHubContext<CommonHub> _hubContext;
    public SignalRService(IHubContext<CommonHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task SendToUser<T>(string topic, string userId, T message)
    {
        await _hubContext.Clients.Group(userId).SendAsync(topic, message);
    }

    public async Task SendToGroup<T>(string topic, string groupId, T message)
    {
        await _hubContext.Clients.Group(groupId).SendAsync(topic, message);
    }

    public async Task SendToAll<T>(string topic, T message)
    {
        await _hubContext.Clients.All.SendAsync(topic, message);
    }

    public async Task SendToUser(string topic, string userId, string message)
    {
        await _hubContext.Clients.Group(userId).SendAsync(topic, message);
    }

    public async Task SendToGroup(string topic, string groupId, string message)
    {
        await _hubContext.Clients.Group(groupId).SendAsync(topic, message);
    }

    public async Task SendToAll(string topic, string message)
    {
        await _hubContext.Clients.All.SendAsync(topic, message);
    }
}
