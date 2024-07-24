namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class SocialPostFavorite : AuditableEntity
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
