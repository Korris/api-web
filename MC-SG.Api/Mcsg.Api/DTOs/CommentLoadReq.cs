using System.ComponentModel;

namespace Mcsg.Api.DTOs
{
    public class CommentLoadReq
    {
        public Guid PostId { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("LastModifiedDate")]
        public string OrderBy { get; set; } = "LastModifiedDate";
    }
}
