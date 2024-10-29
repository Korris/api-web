namespace Mcsg.Document.Api.Requests;

public class FollowPostReq
{
    public Guid PostId { get; set; }
    public Guid SessionId { get; set; }
}
