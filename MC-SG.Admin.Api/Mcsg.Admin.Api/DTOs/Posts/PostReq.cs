using Mcsg.Lib.Model.Enums;

namespace Mcsg.Admin.Api.DTOs.Posts
{
    public class PostListReq : GetByPageReq
    {
        public string HashId { get; set; }
        public PostType? PostType { get; set; }
        public string TitleSearch { get; set; }
    }
}
