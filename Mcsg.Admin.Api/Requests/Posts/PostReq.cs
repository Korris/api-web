namespace Mcsg.Admin.Api.DTOs.Posts
{
    using Common.Core.Enums;

    public class PostListReq : GetByPageReq
    {
        public string HashId { get; set; }
        public PostType? PostType { get; set; }
        public string TitleSearch { get; set; }
    }
}
