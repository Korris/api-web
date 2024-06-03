using Mcsg.Lib.Data.Enums;

namespace Mcsg.Lib.Data.Domain.Entities.Common
{
    public class ReactionBase : AuditableEntity
    {
        public Guid? ParentId { get; set; }
        public Guid TargetId { get; set; }
        public Guid AuthorId { get; set; }
        public ReactionType Type { get; set; }

        public ReactionBase() : base()
        {

        }
    }
}