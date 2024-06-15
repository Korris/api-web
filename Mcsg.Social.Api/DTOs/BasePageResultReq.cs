using System.ComponentModel;

namespace Mcsg.Social.Api.DTOs
{
    public class BasePageResultReq
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("CreatedDate")]
        public string? OrderBy { get; set; }
    }
}
