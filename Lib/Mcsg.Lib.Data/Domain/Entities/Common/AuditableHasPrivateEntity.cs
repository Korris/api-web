namespace Mcsg.Lib.Data.Domain.Entities.Common
{
    using Mcsg.Common.Core.Enums;

    public class AuditableHasPrivateEntity : AuditableEntity
    {
        public PostPermission Permission { get; set; }

        public AuditableHasPrivateEntity() : base()
        {
            Permission = PostPermission.Public;
        }
    }
}