using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Interfaces;

/// <summary>
/// ExpiredSubscription service
/// </summary>
public class ExpiredSubscriptionService : IExpiredSubscriptionService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public ExpiredSubscriptionService(IMcsgContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <returns>Return the result</returns>
    public async Task Run()
    {
        try
        {
            var userIds = await _context.Users.Where(p => p.PremiumDate != null &&
                                                          p.PremiumDate.Value <= DateTime.UtcNow &&
                                                          p.IsExpiredSubscriptionSent != true).Select(p => p.Id).ToListAsync();

            var request = new ExpiredSubscriptionR
            {
                UserIds = userIds
            };

            await SendNotificationAsync(request);

            await _context.Users
             .Where(c => userIds.Any(p => p == c.Id))
             .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsExpiredSubscriptionSent, true));

        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    public async Task<bool> SendNotificationAsync(ExpiredSubscriptionR req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/ExpiredSubscription");

        var url = urlBuilder.ToString();
        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            //var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
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
