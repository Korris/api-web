namespace Mcsg.Common.Domain.Entities
{
    using Common;
    using Core.Enums;

    public class PostReport : AuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public ReasonType ReasonType { get; set; }
        public string? ReasonText { get; set; }
    }
}
