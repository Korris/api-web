using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Api.Services.Interfaces
{
    public interface ISmartCountService
    {
        Task InsertSmartCount(Guid entityId, EntityType type, ActionType actionType);
        Task QueueAddReactionCount(Guid entityId, EntityType type);
        Task QueueRemoveReactionCount(Guid entityId, EntityType type);
    }
}
