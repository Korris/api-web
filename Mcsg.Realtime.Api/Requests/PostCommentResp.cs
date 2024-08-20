namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Enums;
using Dtos;

public class PostCommentResp
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string PostHashId { get; set; }
    public Guid PostCreatedBy { get; set; }
    public DateTime CommentDate { get; set; }
    public string CommentText { get; set; }
    public string Type { get; set; }
    public string ResourceHashId { get; set; }
    public string ResourceUrl { get; set; }
    public string GifId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string UserAvatar { get; set; }
    public List<MentionDto> Mentions { get; set; } = new List<MentionDto>();
    public string? CustomNote { get; set; }
    public float? Order { get; set; }
    public Guid PostIdOfPost { get; set; }
    public PostType PostType { get; set; }
}
