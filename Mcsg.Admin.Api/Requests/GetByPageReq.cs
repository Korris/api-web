using System.ComponentModel;

namespace Mcsg.Admin.Api.Requests
{
    public class GetByPageReq
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("CreatedDate")]
        public string OrderBy { get; set; }
        [DefaultValue(true)]
        public bool OrderByAsc { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
