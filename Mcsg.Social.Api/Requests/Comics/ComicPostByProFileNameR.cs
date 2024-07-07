namespace Mcsg.Social.Api.Requests;

public class ComicPostByProFileNameR : BasePageResultReq
{
    public string? Keyword { get; set; }
    public string? SearchBy { get; set; }
}
