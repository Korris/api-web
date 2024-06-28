namespace Mcsg.Analytic.Api.Models.DTOs
{
    using Common.Core.Enums;

    public class UserAndPostDto
    {
        public Guid? UserId { get; set; }
        public DateTime? ExpiredDateUtc { get; set; }
        public DateOnly? PremiumDate { get; set; }
        public Guid? PostId { get; set; }
        public PostType PostType { get; set; }
        public Guid? AuthorId { get; set; }
    }
}
