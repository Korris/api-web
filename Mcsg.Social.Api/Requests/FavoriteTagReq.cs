using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    public class FavoriteTagReq
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(6)]
        public int PageSize { get; set; }

    }
}
