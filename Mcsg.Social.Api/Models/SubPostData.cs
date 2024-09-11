namespace Mcsg.Social.Api.Models;

public class SubPostData
{
    public Guid Id { get; set; }
    public float Order { get; set; }
    public string HashId { get; set; }
}
public class PostData
{
    public string HashId { get; set; }
    public string Title { get; set; }
}

public class PostDataByPostComment
{
    public Guid CommentId { get; set; }
    public string HashPostId { get; set; }
    public float Order { get; set; }
}

public class ReplyCommentData
{
    public Guid CommentId { get; set; }
    public Guid ReplyCommentId { get; set; }
}
public class ReplyCommentReactionData : ReplyCommentData
{
    public string LocationHashId { get; set; }
    public float Order { get; set; }
}

