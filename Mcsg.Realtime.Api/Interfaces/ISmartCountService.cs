namespace Mcsg.Realtime.Api.Interfaces;

using Common.Core.Enums;

public interface ISmartCountService
{
    Task InsertSmartCount(Guid entityId, EntityType type, ActionType actionType);
    Task QueueAddCommentCount(Guid entityId, EntityType type);
    Task QueueRemoveCommentCount(Guid entityId, EntityType type);
}
