namespace Mcsg.Api.Areas.Realtime.Requests;

using Common.Core.Requests;
using Mcsg.Api.Areas.Realtime.Dtos;

public class PostCommentReq : BaseR
{
    public Guid PostId { get; set; }
    public string CommentText { get; set; }
    public List<MentionDto> Mentions { get; set; } = new List<MentionDto>();
    public string ResourceHashId { get; set; }
    public string GifId { get; set; }
    public string Type { get; set; } // post / subpost
    public string? CustomNote { get; set; }
    public string? ParagraphId { get; set; }
}
