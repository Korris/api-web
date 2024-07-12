namespace Mcsg.Lib.Common.Distributor;

public abstract class BaseDistributor
{
    internal virtual async Task Deliver(DistributedItem item)
    {
        if (await IsAcceptable(item))
        {
            await ApplyAction(item);
        }
    }

    public virtual Task<bool> IsAcceptable(DistributedItem item)
    {
        // The job will not be delivered by default.
        // Developers must override this at the implementation level.
        return Task.FromResult(false);
    }

    public virtual Task ApplyAction(DistributedItem item)
    {
        return Task.CompletedTask;
    }
}
