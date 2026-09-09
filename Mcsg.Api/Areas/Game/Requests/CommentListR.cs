namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Requests;

/// <summary>
/// Paged comments of a post (root comments) or replies of a comment (ParentId set)
/// </summary>
public class CommentListR : PaginatedR
{
    public string? PostHashId { get; set; }
    public Guid? ParentId { get; set; }
}
