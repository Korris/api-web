namespace Mcsg.Realtime.Api.Services
{
    using Common.Core.Enums;
    using Lib.Common.Distributor;
    using Lib.Common.Models;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Models;

    public interface ISmartCountService
    {
        Task InsertSmartCount(Guid entityId, EntityType type, ActionType actionType);
        Task QueueAddCommentCount(Guid entityId, EntityType type);
        Task QueueRemoveCommentCount(Guid entityId, EntityType type);
    }
    public partial class SmartCountService : ISmartCountService
    {
        private readonly IRepository<SmartCountAction> _smartCountActionRepository;
        private readonly DistributeManager _distributeManager;

        public SmartCountService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, DistributeManager distributeManager)
        {
            _smartCountActionRepository = unitOfWork.GetRepository<SmartCountAction>();
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
        public async Task QueueAddCommentCount(Guid entityId, EntityType type)
        {
            await _distributeManager.Deliver(new SmartCountDistributeItem
            {
                Data = new SmartCountEntityData
                {
                    ActionType = ActionType.COMMENT,
                    EntityId = entityId,
                    EntityType = type,
                    IsRemove = false
                }
            });
        }
        public async Task QueueRemoveCommentCount(Guid entityId, EntityType type)
        {
            await _distributeManager.Deliver(new SmartCountDistributeItem
            {
                Data = new SmartCountEntityData
                {
                    ActionType = ActionType.COMMENT,
                    EntityId = entityId,
                    EntityType = type,
                    IsRemove = true
                }
            });
        }

    }
}
