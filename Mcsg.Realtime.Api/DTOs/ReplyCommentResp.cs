using Mcsg.Realtime.Api.DTOs.Mention;

namespace Mcsg.Realtime.Api.DTOs
{
    public class ReplyCommentResp
    {
        public Guid Id { get; set; }
        public Guid ReplyToCommentId { get; set; }
        public string ReplyText { get; set; }
        public DateTime ReplyDate { get; set; }
        public string Type { get; set; }
        public Guid PostId { get; set; }
        public string PostHashId { get; set; }
        public Guid PostCreatedBy { get; set; }
        public string ResourceHashId { get; set; }
        public string ResourceUrl { get; set; }
        public string GifId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid? QuoteId { get; set; }
        public string AuthorName { get; set; }
        public string UserAvatar { get; set; }
        public List<MentionDto> Mentions { get; set; } = new List<MentionDto>();
    }
}
