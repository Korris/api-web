using System.ComponentModel;

namespace Mcsg.Api.DTOs
{
    public class FavoriteTagReq
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; }
        [DefaultValue(6)]
        public int PageSize { get; set; }

    }
}
