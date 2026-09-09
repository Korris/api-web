namespace Mcsg.Api.Areas.Game.Models;

/// <summary>
/// Game post comment / reply
/// </summary>
public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string? PostHashId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Body { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? UserName { get; set; }
    public string? ProfileId { get; set; }
    public string? UserAvatar { get; set; }
    public bool IsCurrentUserAuthor { get; set; }
    public int ReplyCount { get; set; }
    public ReactionSummaryResponse? Reaction { get; set; }
}
