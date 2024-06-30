using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Requests
{
    public class ReportPostReq
    {
        public Guid PostId { get; set; }
        public ReasonType ReasonType { get; set; }
        public string ReasonText { get; set; }
    }
}
