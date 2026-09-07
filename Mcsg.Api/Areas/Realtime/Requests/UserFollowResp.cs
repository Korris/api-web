namespace Mcsg.Api.Areas.Realtime.Requests;

public class UserFollowResp
{
    public Guid CreatedByUserId { get; set; }
    public Guid FollowedId { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool Status { get; set; }
    public string CreatedByUserName { get; set; }
    public string CreatedByUserAvata { get; set; }
}
