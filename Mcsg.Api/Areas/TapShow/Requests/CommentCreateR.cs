namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Create a comment (ParentId null) or a reply (ParentId = root comment id) on a TapShow post
/// </summary>
public class CommentCreateR : BaseR
{
    public string? PostHashId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Body { get; set; }
}
