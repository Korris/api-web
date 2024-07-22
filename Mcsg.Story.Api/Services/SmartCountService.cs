namespace Mcsg.Story.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Domain.Entities;
using Interfaces;
using Lib.Common.Models;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;

public partial class SmartCountService : ISmartCountService
{
    private readonly IRepository<SmartCountAction> _smartCountActionRepository;
    private readonly IUnitOfWork _unitOfWork;
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
