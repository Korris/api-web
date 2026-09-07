namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.Models;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Interfaces;

public partial class SmartCountService : BaseS, ISmartCountService
{
    /// <summary>
    /// Initialize
    /// </summary>
    public SmartCountService(IMcsgContext context, DistributeManager distributeManager) : base(context)
    {
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
        await _context.SmartCountActions.AddAsync(smartCountPostAction);
        await _context.SaveChangesAsync(default);
    }

    public async Task QueueAddCommentCount(Guid entityId, EntityType type)
    {
        await _distributeManager.Deliver(new SmartCountDistributeDto
        {
            Data = new SmartCountEntityData
            {
                ActionType = ActionType.Comment,
                EntityId = entityId,
                EntityType = type,
                IsRemove = false
            }
        });
    }

    public async Task QueueRemoveCommentCount(Guid entityId, EntityType type)
    {
        await _distributeManager.Deliver(new SmartCountDistributeDto
        {
            Data = new SmartCountEntityData
            {
                ActionType = ActionType.Comment,
                EntityId = entityId,
                EntityType = type,
                IsRemove = true
            }
        });
    }

    #region -- Fields --

    private readonly DistributeManager _distributeManager;

    #endregion
}
