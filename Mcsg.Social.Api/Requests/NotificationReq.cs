using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    public class NotificationReq
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
}
