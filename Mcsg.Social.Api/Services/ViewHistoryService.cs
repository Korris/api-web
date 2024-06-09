using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Services
{
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
