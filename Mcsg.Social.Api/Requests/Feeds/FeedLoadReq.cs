namespace Mcsg.Social.Api.Requests;

public class FeedLoadReq : BasePageResultR
{
    public string? ProfileName { get; set; }
    public string? UserName { get; set; }
}
