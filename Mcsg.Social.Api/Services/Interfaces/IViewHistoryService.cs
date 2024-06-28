namespace Mcsg.Social.Api.Services.Interfaces
{
    using Common.Core.Enums;

    public interface IViewHistoryService
    {
        Task PrepareAddView(Guid userId, Guid entityId, EntityType type, string ipAddress, EntitySubType? subType);
        Task QueueAddView(Guid userId, Guid entityId, EntityType type, string ipAddress, EntitySubType? subType);
    }
}
