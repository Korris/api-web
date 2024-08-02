namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class BaseReaction : AuditableEntity
{
    public Guid? ParentId { get; set; }
    public Guid TargetId { get; set; }
    public Guid AuthorId { get; set; }
    public ReactionType Type { get; set; }
}