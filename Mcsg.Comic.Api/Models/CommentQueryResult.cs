namespace Mcsg.Comic.Api.Models;

using Common.SeedWork.Enums;

public class CommentQueryResult
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public Guid ReplyId { get; set; }
    public string ReplyAuthorName { get; set; } = string.Empty;
    public string ReplyUserAvatar { get; set; } = string.Empty;
    public string ReplyBody { get; set; } = string.Empty;
    public DateTime ReplyLastCreatedDate { get; set; }
    public string ReplyResourceHashId { get; set; } = string.Empty;
    public string ReplyResourceName { get; set; } = string.Empty;
    public string ReplyResourceUrl { get; set; } = string.Empty;
    public Guid ReplyQuoteId { get; set; } = Guid.Empty;
    public string GifId { get; set; } = string.Empty;
    public string ReplyGifId { get; set; } = string.Empty;
    public int TotalRecord { get; set; } = 0;
}
