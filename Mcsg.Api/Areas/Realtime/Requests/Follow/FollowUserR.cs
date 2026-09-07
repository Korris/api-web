namespace Mcsg.Api.Areas.Realtime.Requests;

using Common.Core.Requests;

public class FollowUserR : BaseR
{
    public Guid CreatedByUserId { get; set; }
    public Guid FollowedId { get; set; }
}
