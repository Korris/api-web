using Mcsg.Lib.Data.Enums;

namespace Mcsg.Lib.Data.Domain.Entities.Common
{
    public class AuditableHasPrivateEntity : AuditableEntity
    {
        public PostPermission Permission { get; set; }

        public AuditableHasPrivateEntity() : base()
        {
            Permission = PostPermission.PUBLIC;
        }
    }
}