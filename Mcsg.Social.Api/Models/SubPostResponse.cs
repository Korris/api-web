using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Models
{
    public class SubPostBasic
    {
        public Guid Id { get; set; }
        public string HashId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public PostPermission Permission { get; set; }
        public PostStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UserId { get; set; }
        public string CreatorNote { get; set; }
        public bool IsExclusive { get; set; }
    }
    public class SubPostResponse : SubPostBasic
    {
        public string ThumbnailUrl { get; set; }
        public List<UploadFileResponse> Files { get; set; } = new List<UploadFileResponse>();
        public string Body { get; set; }
    }
    public class SubPostQueryDbResponse : SubPostResponse
    {
        public List<UploadFileQueryDbResponse> FileDbs { get; set; }
    }

    public class SubPostFeedResponse : FeedResponse
    {
        public string HashIdPost { get; set; }
        /// <summary>
        /// File Information
        /// </summary>
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ResourceName { get; set; }
        public string ShareUrl { get; set; }
        ///
        public ResourceType ResourceType { get; set; }
        public string PrevSubPostHashId { get; set; }
        public string NextSubPostHashId { get; set; }
    }
}
