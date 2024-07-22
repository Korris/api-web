namespace Mcsg.Common.Domain.Entities.Common
{
    using Core.Enums;

    public class AuditableHasPrivateEntity : AuditableEntity
    {
        public PostPermission Permission { get; set; }

        public AuditableHasPrivateEntity() : base()
        {
            Permission = PostPermission.Public;
        }
    }
}