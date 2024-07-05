using Mcsg.Realtime.Api.DTOs.Mention;

namespace Mcsg.Realtime.Api.DTOs
{
    public class PostCommentReq
    {
        public Guid PostId { get; set; }
        public string CommentText { get; set; }
        public List<MentionDto> Mentions { get; set; } = new List<MentionDto>();
        public string ResourceHashId { get; set; }
        public string GifId { get; set; }
        public string Type { get; set; } // post / subpost
    }
}
