using Mcsg.Lib.Model.Enums;
using System.ComponentModel;

namespace Mcsg.Api.DTOs
{
    public class TodayTrendingTagReq
    {
        [DefaultValue(null)]
        public PostType? PostType { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(6)]
        public int PageSize { get; set; }
    }
}
