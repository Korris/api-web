using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class PostReport : AuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public ReasonType ReasonType { get; set; }
        public string ReasonText { get; set; }
    }
}
