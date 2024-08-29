namespace Mcsg.Realtime.Api.Interfaces;

using Requests;

public interface IFollowPostService
{
    Task SendPostFollowNotification(FollowPostReq req);
}
