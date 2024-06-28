namespace Mcsg.Social.Api.Services
{
    using Common.Core.Enums;
    using Lib.Common.Distributor;
    using Lib.Common.Models;
    using Models;
    using Services.Interfaces;

    public partial class ViewHistoryService : IViewHistoryService
    {
        private readonly DistributeManager _distributeManager;

        public ViewHistoryService(DistributeManager distributeManager)
        {
            _distributeManager = distributeManager;
        }

        public async Task PrepareAddView(Guid userId, Guid entityId, EntityType type, string ipAddress, EntitySubType? subType)
        {
            await _distributeManager.Deliver(new ViewHistoryDistributeItem
            {
                Data = new ViewHistoryData
                {
                    UserId = userId,
                    EntityId = entityId,
                    EntityType = type,
                    IdAddress = ipAddress,
                    SubType = subType
                }
            });
        }

        public async Task QueueAddView(Guid userId, Guid entityId, EntityType type, string ipAddress, EntitySubType? subType)
        {
            await _distributeManager.Deliver(new ViewHistoryDistributeItem
            {
                Data = new ViewHistoryData
                {
                    UserId = userId,
                    EntityId = entityId,
                    EntityType = type,
                    IdAddress = ipAddress,
                    SubType = subType
                }
            });
        }
    }
}
