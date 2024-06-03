using System.ComponentModel;

namespace Mcsg.Api.DTOs
{
    public class TopPostReq : BasePageResultReq
    {
    }
    public class TopPostRecommendedReq : TopPostReq
    {
        [DefaultValue(3)]
        public int PageSize { get; set; }
    }
}
