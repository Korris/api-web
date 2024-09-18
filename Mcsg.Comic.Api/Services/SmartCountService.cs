namespace Mcsg.Comic.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Domain.Entities;
using Interfaces;
using Lib.Common.Models;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;

public class SmartCountService : ISmartCountService
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

    public async Task QueueAddReactionCount(Guid entityId, EntityType type)
    {
        await _distributeManager.Deliver(new SmartCountDistributeItem
        {
            Data = new SmartCountEntityData
            {
                ActionType = ActionType.Reaction,
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
                ActionType = ActionType.Reaction,
                EntityId = entityId,
                EntityType = type,
                IsRemove = true
            }
        });
    }

}
