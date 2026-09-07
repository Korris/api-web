namespace Mcsg.Api.Areas.Document.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Models;
using Mcsg.Api.Areas.Document.Interfaces;
using Mcsg.Api.Areas.Document.Models;

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
