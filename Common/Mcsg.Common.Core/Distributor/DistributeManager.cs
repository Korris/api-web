using System.Reflection;

namespace Mcsg.Common.Core.Distributor;

/// <summary>
/// Distribute manager
/// </summary>
public sealed class DistributeManager
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="assembly">Assembly</param>
    /// <param name="provider">Service provider</param>
    public DistributeManager(Assembly assembly, IServiceProvider provider)
    {
        _distrbutors = LoadDistributor(assembly, provider);
    }

    /// <summary>
    /// Deliver
    /// </summary>
    /// <param name="item">Distributed item</param>
    /// <returns>Return the result</returns>
    public async Task Deliver(DistributedItem item)
    {
        foreach (var i in _distrbutors)
        {
            await i.Deliver(item).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Load distributor
    /// </summary>
    /// <param name="assembly">Assembly</param>
    /// <param name="provider">Service provider</param>
    /// <returns>Return the result</returns>
    private IList<BaseDistributor> LoadDistributor(Assembly assembly, IServiceProvider provider)
    {
        var res = new List<BaseDistributor>();

        var baseType = typeof(BaseDistributor);
        var derivedTypes = assembly.GetTypes().Where(p => p.IsClass && !p.IsAbstract && baseType.IsAssignableFrom(p)).ToList();
        foreach (var i in derivedTypes)
        {
            var instance = Activator.CreateInstance(i, provider) as BaseDistributor;
            if (instance == null)
            {
                continue;
            }

            res.Add(instance);
        }

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Distributor
    /// </summary>
    private readonly IList<BaseDistributor> _distrbutors;

    #endregion
}
