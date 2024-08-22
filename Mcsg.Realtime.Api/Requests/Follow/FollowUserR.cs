namespace Mcsg.Realtime.Api.Requests;

public class FollowUserR
{
    public Guid CreatedByUserId { get; set; }
    public Guid FollowedId { get; set; }
}
