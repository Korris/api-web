using Mcsg.Lib.Data.Domain.Entities.Common;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class PostFavorite : AuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
