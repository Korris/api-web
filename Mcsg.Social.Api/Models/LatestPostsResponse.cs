using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Models
{
    public class LatestPostsResponse
    {
        public string HashId { get; set; }
        public PostType Type { get; set; }
    }

    public class ListIdForHomePage
    {
        public int TotalItems { get; set; }
        public List<LatestPostsResponse> LatestPostsResponses { get; set; }
    }
}