namespace Mcsg.Social.Api.Models
{
    using Common.Core.Enums;

    public class FeedBoxResponse : FeedBox
    {
        public List<SubPostResponse>? SubPosts { get; set; } = new List<SubPostResponse>();
        public List<ResourceResponse>? Resources { get; set; } = new List<ResourceResponse>();
        public MetaDataResponse? MetaData { get; set; }
        public PostLinkResponse? Link { get; set; }

    }

    public class PostLinkFeedBoxResponse
    {
        public string? HashId { get; set; } = "";
        public string? Url { get; set; } = "";
        public PostLinkType Type { get; set; } = 0;
    }

    public class FeedBoxQueryResponse : FeedBox
    {
        public string? SubPosts { get; set; }
        public string? Resources { get; set; }
        public string? MetaDatas { get; set; }
    }

    public class FeedBox
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UserId { get; set; }
        public string? UserAvatar { get; set; }
        public string? Body { get; set; }
        public string? HashId { get; set; }
        public string? FullName { get; set; }
        public string? ProfileId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int TotalResources { get; set; }
        public PostType Type { get; set; }
        public string? Link { get; set; }
    }
}
