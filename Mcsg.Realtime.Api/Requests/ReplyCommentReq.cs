using Mcsg.Realtime.Api.DTOs.Mention;

namespace Mcsg.Realtime.Api.DTOs
{
    public class ReplyCommentReq
    {
        public Guid ReplyToCommentId { get; set; }
        public string ReplyText { get; set; }
        public List<MentionDto> Mentions { get; set; } = new List<MentionDto>();
        public string Type { get; set; }
        public Guid PostId { get; set; }
        public string ResourceHashId { get; set; }
        public string GifId { get; set; }
        public Guid? QuoteId { get; set; }
    }
}
