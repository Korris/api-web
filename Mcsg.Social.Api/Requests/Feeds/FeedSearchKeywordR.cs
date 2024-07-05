using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    public class FeedSearchKeywordR
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("CreatedDate")]
        public string? OrderBy { get; set; }
    }
}
