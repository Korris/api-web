namespace Mcsg.Realtime.Api.Requests
{
    public class DeleteCommentReq
    {
        public Guid CommentId { get; set; }
        public string Type { get; set; }
        public string MicroService { get; set; } = Common.Core.Enums.MicroService.Social.ToString();
    }
}
