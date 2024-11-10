using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class UserRelation : AuditableEntity
{
    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
    public UserRelationStatus Status { get; set; }

    [ForeignKey("UserId1")]
    [InverseProperty("UserRelationUserId1Navigations")]
    public virtual User UserId1Navigation { get; set; } = null!;

    [ForeignKey("UserId2")]
    [InverseProperty("UserRelationUserId2Navigations")]
    public virtual User UserId2Navigation { get; set; } = null!;
}
