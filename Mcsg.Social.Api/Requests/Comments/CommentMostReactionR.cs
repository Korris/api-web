namespace Mcsg.Social.Api.Requests;

public class CommentMostReactionR : BasePageResultReq
{
    public string? HashPostId { get; set; }
    public bool IsGetTotalPostComment { get; set; }

}
