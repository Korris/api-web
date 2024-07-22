namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;

public interface ISmartCountService
{
    Task InsertSmartCount(Guid entityId, EntityType type, ActionType actionType);
    Task QueueAddReactionCount(Guid entityId, EntityType type);
    Task QueueRemoveReactionCount(Guid entityId, EntityType type);
}
