using System.ComponentModel;

namespace Mcsg.Social.Api.Requests
{
    public class ComicChapterListR : BasePageResultReq
    {
        [DefaultValue("Order")]
        public string? OrderBy { get; set; }
    }
}
