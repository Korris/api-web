using System.ComponentModel;

namespace Mcsg.Lib.Common.Models
{
    public class PaginatedRequest
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("CreatedDate")]
        public string OrderBy { get; set; }
    }
}
