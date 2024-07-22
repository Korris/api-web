namespace Mcsg.Common.Domain.Entities
{
    using Common;

    public class PostFavorite : AuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
