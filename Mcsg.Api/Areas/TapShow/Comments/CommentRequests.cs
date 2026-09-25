using System.ComponentModel;

namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Core.Enums;
using Common.Core.Requests;

// Requests cloned 1:1 from Areas/Story/Requests (Comments / Reactions / Feeds)

public class CommentLoadR : PaginatedR
{
    public Guid PostId { get; set; }

    [DefaultValue("CreatedOn")]
    public new string? OrderBy { get; set; } = "CreatedOn";
}

public class CommentMostReactionR : PaginatedR
{
    public string? HashPostId { get; set; }
    public bool IsGetTotalPostComment { get; set; }
}

public class CommentReplyByCommentR : PaginatedR
{
    public Guid CommentId { get; set; }

    /// <summary>
    /// Kept for request compatibility with Story; TapShow only has post comments
    /// </summary>
    public bool IsSubPost { get; set; }
}

public class CommentCheckPostExistedR : BaseR
{
    public Guid? PostId { get; set; }

    /// <summary>
    /// Kept for request compatibility with Story; TapShow only has post comments
    /// </summary>
    public bool IsSubPost { get; set; }
}

public class FeedReactionByTargetR : PaginatedR
{
    public string? Type { get; set; }
}

public class ReactionReactR : IdBaseR
{
    public ReactionReactR() { }

    public ReactionReactR(HttpContext hc) : base(hc) { }

    public Guid TargetId { get; set; }

    public ReactionType Type { get; set; }

    public bool? IsReply { get; set; }
}
