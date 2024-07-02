namespace Mcsg.Social.Api.Requests
{
    using Common.Core.Enums;
    using Lib.Data.Enums;

    public class ChapterPostReq
    {
        public string? Title { get; set; }
        public string? Name { get; set; }
        #region Setting
        public bool IsPublicNow { get; set; }
        public PostPermission Permission { get; set; }
        public PostStatus Status { get; set; }
        public DateTime? PublishDate { get; set; }
        //public string CreatorNote { get; set; }
        public bool IsEnableComment { get; set; }
        #endregion
    }
    public class ChapterComicReq : ChapterPostReq
    {
        public List<ResourcePostReq> Files { get; set; }
    }
    public class ChapterStoryReq : ChapterPostReq
    {
        public string? Body { get; set; }
    }
    public class ChapterOrderSwapReq
    {
        public int Order1 { get; set; }
        public int Order2 { get; set; }
    }

}
