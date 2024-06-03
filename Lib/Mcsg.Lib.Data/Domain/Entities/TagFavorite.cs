using Mcsg.Lib.Data.Domain.Entities.Common;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class TagFavorite : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Guid UserId { get; set; }
    }
}
