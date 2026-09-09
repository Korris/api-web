namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Requests;

/// <summary>
/// Create a comment (ParentId null) or a reply (ParentId = root comment id) on a game post
/// </summary>
public class CommentCreateR : BaseR
{
    public string? PostHashId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Body { get; set; }
}
