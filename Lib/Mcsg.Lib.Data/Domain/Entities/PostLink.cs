using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class PostLink : AuditableEntity
    {
        public Guid PostId { get; set; }
        public string? HashId { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public PostLinkType Type { get; set; }
    }
}
