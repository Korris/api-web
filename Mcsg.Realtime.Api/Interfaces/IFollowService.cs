namespace Mcsg.Realtime.Api.Interfaces;

using Requests;

public interface IFollowService
{
    Task<UserFollowResp> FollowUser(FollowUserR request);
}
