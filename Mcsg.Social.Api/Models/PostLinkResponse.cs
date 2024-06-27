using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Models
{
    public class PostLinkResponse
    {
        public string? HashId { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
    }
    public class PostLinkDb
    {
        public string? HashId { get; set; }
        public string? Url { get; set; }
        public PostLinkType Type { get; set; }
    }
}
