namespace Mcsg.Common.Domain.Entities
{
    using Common;

    public class TagFavorite : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Guid UserId { get; set; }
    }
}
