namespace Mcsg.Story.Api.Models;

using Common.SeedWork.Enums;

public class CommentQueryModel
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid PostId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public string GifId { get; set; } = string.Empty;
    public int CommentLevel { get; set; } = 0;
    public Guid? QuoteId { get; set; }
    public string? CustomNote { get; set; }
    public int ReplyCount { get; set; } = 0;
}
