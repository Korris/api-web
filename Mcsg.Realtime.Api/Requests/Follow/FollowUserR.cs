namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Requests;

public class FollowUserR : BaseR
{
    public Guid CreatedByUserId { get; set; }
    public Guid FollowedId { get; set; }
}
