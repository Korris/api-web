namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Mcsg.Api.Areas.Realtime.Requests;

public interface IFollowPostService
{
    Task SendPostFollowNotification(FollowPostReq req);
}
