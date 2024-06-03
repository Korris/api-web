namespace Mcsg.Api.Models
{
    public class FavoritePostResponse
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
    }
}
