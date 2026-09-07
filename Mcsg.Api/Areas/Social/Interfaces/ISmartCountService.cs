namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Core.Enums;

public interface ISmartCountService
{
    Task QueueAddReactionCount(Guid entityId, EntityType type);
    Task QueueRemoveReactionCount(Guid entityId, EntityType type);
}
