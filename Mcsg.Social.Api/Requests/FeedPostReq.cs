namespace Mcsg.Social.Api.Requests
{
    public class FeedPostReq
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ThumbnailUrl { get; set; }
        public MetaDataReq MetaData { get; set; }
        public List<string> Tags { get; set; }
        public List<ResourcePostReq> Files { get; set; }
        public Guid? SoundId { get; set; }
        public string? CustomNote { get; set; }
    }
}
