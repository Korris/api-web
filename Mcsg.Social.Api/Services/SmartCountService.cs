using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class SmartCountService : ISmartCountService
    {
        private readonly IRepository<SmartCountAction> _smartCountActionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureBlobStorageQueueService _queueService;
        private readonly DistributeManager _distributeManager;

        public SmartCountService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, DistributeManager distributeManager)
        {
            _smartCountActionRepository = unitOfWork.GetRepository<SmartCountAction>();
            _unitOfWork = unitOfWork;
            _distributeManager = distributeManager;
        }
        public async Task InsertSmartCount(Guid entityId, EntityType type, ActionType actionType)
        {
            var smartCountPostAction = new SmartCountAction
            {
                ActionType = actionType,
                Count = 0,
                EntityType = type,
                EntityId = entityId
            };
            await _smartCountActionRepository.InsertAsync(smartCountPostAction);
        }
        public async Task QueueAddReactionCount(Guid entityId, EntityType type)
        {
            await _distributeManager.Deliver(new SmartCountDistributeItem
            {
                Data = new SmartCountEntityData
                {
                    ActionType = ActionType.REACTION,
                    EntityId = entityId,
                    EntityType = type,
                    IsRemove = false
                }
            });
        }
        public async Task QueueRemoveReactionCount(Guid entityId, EntityType type)
        {
            await _distributeManager.Deliver(new SmartCountDistributeItem
            {
                Data = new SmartCountEntityData
                {
                    ActionType = ActionType.REACTION,
                    EntityId = entityId,
                    EntityType = type,
                    IsRemove = true
                }
            });
        }

    }
}
