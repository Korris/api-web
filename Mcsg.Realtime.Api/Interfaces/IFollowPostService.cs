using Mcsg.Realtime.Api.Requests;

namespace Mcsg.Realtime.Api.Interfaces;
public interface IFollowPostService
{
    Task SendPostFollowNotification(FollowPostReq req);
}
