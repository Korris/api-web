namespace Mcsg.Api.Areas.Comic.Interfaces;

using Common.Core.Enums;

public interface ISmartCountService
{
    Task QueueAddReactionCount(Guid entityId, EntityType type);
    Task QueueRemoveReactionCount(Guid entityId, EntityType type);
}
