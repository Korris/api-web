using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    public class TopPostReq : BasePageResultReq
    {
    }
    public class TopPostRecommendedReq : TopPostReq
    {
        [DefaultValue(3)]
        public int PageSize { get; set; }
    }

    public class PostByProFileNameInput : BasePageResultReq
    {
        public string? Keyword { get; set; }
        public string? SearchBy { get; set; }
    }

    public class PostByTagNameInput : BasePageResultReq
    {
        public string? TagName { get; set; }
    }
}
