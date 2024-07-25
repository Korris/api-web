namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class StoryFollowedPost : AuditableEntity
{
    public Guid PostId { get; set; }
}