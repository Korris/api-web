namespace Mcsg.Comic.Api.Requests;

public class CommentMostReactionR : BasePageResultR
{
    public string? HashPostId { get; set; }
    public bool IsGetTotalPostComment { get; set; }

}
