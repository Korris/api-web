using Mcsg.Lib.Model.Enums;
using System.ComponentModel;

namespace Mcsg.Social.Api.DTOs
{
    public class PopularTagReq
    {
        [DefaultValue(null)]
        public PostType? PostType { get; set; }
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
}
