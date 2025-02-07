namespace Mcsg.Function.Job.Services;

using Common.Core.Extensions;
using Common.Domain;
using Interfaces;

/// <summary>
/// RemindExpiredSubscription service
/// </summary>
public class RemindExpiredSubscriptionService : IRemindExpiredSubscriptionService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public RemindExpiredSubscriptionService(IMcsgContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <returns>Return the result</returns>
    public async Task Run()
    {
        try
        {
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
