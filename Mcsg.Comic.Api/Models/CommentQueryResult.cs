namespace Mcsg.Comic.Api.Models;

public class CommentQueryResult
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime? ModifiedOn { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public Guid ReplyId { get; set; }
    public string ReplyAuthorName { get; set; } = string.Empty;
    public string ReplyUserAvatar { get; set; } = string.Empty;
    public string ReplyBody { get; set; } = string.Empty;
    public DateTime? ReplyLastModifiedDate { get; set; }
    public string ReplyResourceHashId { get; set; } = string.Empty;
    public string ReplyResourceName { get; set; } = string.Empty;
    public string ReplyResourceUrl { get; set; } = string.Empty;
    public Guid ReplyQuoteId { get; set; } = Guid.Empty;
    public string GifId { get; set; } = string.Empty;
    public string ReplyGifId { get; set; } = string.Empty;
    public int TotalRecord { get; set; } = 0;
}
