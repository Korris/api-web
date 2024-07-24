namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class StoryPostFavorite : AuditableEntity
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
