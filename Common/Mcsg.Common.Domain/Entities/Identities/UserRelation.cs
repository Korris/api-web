namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class UserRelation : AuditableEntity
{
    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
    public UserRelationStatus Status { get; set; }
}