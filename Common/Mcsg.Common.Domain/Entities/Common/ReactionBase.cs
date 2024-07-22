namespace Mcsg.Common.Domain.Entities.Common
{
    using Core.Enums;

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