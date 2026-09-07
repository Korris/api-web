namespace Mcsg.Api.Areas.Comic.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Models;
using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Comic.Models;

public class SmartCountService : ISmartCountService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="distributeManager"></param>
    public SmartCountService(DistributeManager distributeManager)
    {
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

    #region -- Fields --

    private readonly DistributeManager _distributeManager;

    #endregion
}
