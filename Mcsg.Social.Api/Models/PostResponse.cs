using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Models
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string HashId { get; set; }
        public Guid UserId { get; set; }
        public string ProfileId { get; set; }
        public string AuthorName { get; set; }
        public bool IsCurrentUserIsAuthor { get; set; }
        public string ThumbnailUrl { get; set; }
        public string UserAvatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public PostType Type { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public PostStatus Status { get; set; }
        public PostPermission Permission { get; set; }
        public List<SubPostResponse> SubPosts { get; set; } = new List<SubPostResponse>();
        public string CustomNote { get; set; }
    }
}
