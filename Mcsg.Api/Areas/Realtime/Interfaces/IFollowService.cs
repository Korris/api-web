namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Mcsg.Api.Areas.Realtime.Requests;

public interface IFollowService
{
    Task<UserFollowResp> FollowUser(FollowUserR request);
}
