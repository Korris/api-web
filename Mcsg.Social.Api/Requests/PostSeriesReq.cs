using Mcsg.Lib.Data.Enums;
using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    public class PostSeriesReq
    {
        public string Title { get; set; }
        public string? Summary { get; set; }
        public string ThumbnailUrl { get; set; }
        public string CoverUrl { get; set; }
        public bool IsCurrentUserIsAuthor { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public bool IsMature { get; set; } = false;
        public PostPermission Permission { get; set; }
        public List<string> Tags { get; set; }
        public bool IsSaveAndPublish { get; set; }
    }
    public class PostUpdateSeriesReq : PostSeriesReq
    {
        public bool IsCompleted { get; set; }
    }
    public class PostListSeriesReq : BasePageResultReq
    {
        public string? HashTag { get; set; }
        public bool IsFavorite { get; set; }
    }
    public class MostReactionCommentInput : BasePageResultReq
    {
        public string? HashPostId { get; set; }
        public bool IsGetTotalPostComment { get; set; }

    }
    public class ReplyByCommentInput : BasePageResultReq
    {
        public Guid CommentId { get; set; }
        public bool IsSubPost { get; set; }
    }
    public class ChapterListReq : BasePageResultReq
    {
        [DefaultValue("Order")]
        public string OrderBy { get; set; }
    }
}
