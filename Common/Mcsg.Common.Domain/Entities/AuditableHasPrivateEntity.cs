namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class AuditableHasPrivateEntity : AuditableEntity
{
    public PostPermission Permission { get; set; }

    public AuditableHasPrivateEntity() : base()
    {
        Permission = PostPermission.Public;
    }
}