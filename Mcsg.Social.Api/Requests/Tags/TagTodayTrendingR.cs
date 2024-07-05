using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    using Common.Core.Enums;

    public class TagTodayTrendingR
    {
        [DefaultValue(null)]
        public PostType? PostType { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(6)]
        public int PageSize { get; set; }
    }
}
