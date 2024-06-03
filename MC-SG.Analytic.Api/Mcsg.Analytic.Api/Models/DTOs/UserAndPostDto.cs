namespace Mcsg.Analytic.Api.Models.DTOs
{
    public class UserAndPostDto
    {
        public Guid? UserId { get; set; }
        public DateTime? ExpiredDateUtc { get; set; }
        public DateOnly? PremiumDate { get; set; }
        public Guid? PostId { get; set; }
        public Lib.Model.Enums.PostType PostType { get; set; }
        public Guid? AuthorId { get; set; }
    }
}
