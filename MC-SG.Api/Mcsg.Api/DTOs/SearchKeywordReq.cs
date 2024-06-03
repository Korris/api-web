using System.ComponentModel;

namespace Mcsg.Api.DTOs
{
    public class SearchKeywordReq
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("CreatedDate")]
        public string OrderBy { get; set; }
    }
}
