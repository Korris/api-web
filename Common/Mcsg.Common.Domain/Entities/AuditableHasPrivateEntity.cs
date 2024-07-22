namespace Mcsg.Common.Domain.Entities;

using Core.Enums;

public class AuditableHasPrivateEntity : AuditableEntity
{
    public PostPermission Permission { get; set; }

    public AuditableHasPrivateEntity() : base()
    {
        Permission = PostPermission.Public;
    }
}