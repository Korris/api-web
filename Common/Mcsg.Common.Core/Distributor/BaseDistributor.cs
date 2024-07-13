namespace Mcsg.Common.Core.Distributor;

/// <summary>
/// Base distributor
/// </summary>
public abstract class BaseDistributor
{
    /// <summary>
    /// Deliver
    /// </summary>
    /// <param name="item">Distributed item</param>
    /// <returns>Return the result</returns>
    internal virtual async Task Deliver(DistributedItem item)
    {
        if (await IsAcceptable(item))
        {
            await ApplyAction(item);
        }
    }

    /// <summary>
    /// Is acceptable
    /// </summary>
    /// <param name="item">Distributed item</param>
    /// <returns>Return the result</returns>
    public virtual Task<bool> IsAcceptable(DistributedItem item)
    {
        // The job will not be delivered by default.
        // Developers must override this at the implementation level.
        return Task.FromResult(false);
    }

    /// <summary>
    /// Apply action
    /// </summary>
    /// <param name="item">Distributed item</param>
    /// <returns>Return the result</returns>
    public virtual Task ApplyAction(DistributedItem item)
    {
        return Task.CompletedTask;
    }
}
