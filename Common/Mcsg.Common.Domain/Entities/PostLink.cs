namespace Mcsg.Lib.Data.Domain.Entities
{
    using Lib.Data.Domain.Entities.Common;
    using Mcsg.Common.Core.Enums;

    public class PostLink : AuditableEntity
    {
        public Guid PostId { get; set; }
        public string? HashId { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public PostLinkType Type { get; set; }
    }
}
