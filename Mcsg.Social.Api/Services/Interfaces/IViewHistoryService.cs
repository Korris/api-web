using Mcsg.Lib.Model.Enums;

namespace Mcsg.Api.Services.Interfaces
{
    public interface IViewHistoryService
    {
        Task PrepareAddView(Guid userId, Guid entityId, EntityType type, string ipAddress, EntitySubType? subType);
        Task QueueAddView(Guid userId, Guid entityId, EntityType type, string ipAddress, EntitySubType? subType);
    }
}
