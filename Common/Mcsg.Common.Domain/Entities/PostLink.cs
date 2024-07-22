namespace Mcsg.Common.Domain.Entities
{
    using Common;
    using Core.Enums;

    public class PostLink : AuditableEntity
    {
        public Guid PostId { get; set; }
        public string? HashId { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public PostLinkType Type { get; set; }
    }
}
