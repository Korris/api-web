namespace Mcsg.Admin.Api.Requests
{
    using Common.Core.Enums;

    public class PostListReq : GetByPageR
    {
        public string HashId { get; set; }
        public PostType? PostType { get; set; }
        public string TitleSearch { get; set; }
    }
}
