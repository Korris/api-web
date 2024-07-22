namespace Mcsg.Social.Api.Models;

public class ReplyResponse
{
    public int TotalReply { get; set; } = 0;
    public List<ReplyData> Data { get; set; } = new List<ReplyData>();
}
public class ReplyData
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime? ModifiedOn { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public string GifId { get; set; } = string.Empty;
    public Guid? QuoteId { get; set; } = null;
}
