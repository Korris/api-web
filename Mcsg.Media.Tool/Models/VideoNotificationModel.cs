namespace Mcsg.Media.Tool.Models
{
    public class VideoNotificationModel
    {
        public Guid Id { get; set; }
        public string HashId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Guid PostId { get; set; }
        public string PostHashId { get; set; }
    }
}
