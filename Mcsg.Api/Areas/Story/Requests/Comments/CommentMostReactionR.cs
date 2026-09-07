namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class CommentMostReactionR : PaginatedR
{
    public string? HashPostId { get; set; }
    public bool IsGetTotalPostComment { get; set; }

}
