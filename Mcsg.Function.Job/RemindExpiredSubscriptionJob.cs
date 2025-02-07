using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Mcsg.Function.Job;

using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Interfaces;

public class RemindExpiredSubscriptionJob : IJob
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    public RemindExpiredSubscriptionJob(IMcsgContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            /// This job will run at 12AM 
            /// Example today is 6/2 we will get list user in 6/2, 7/2 and 8/2
            var frDate = DateTime.UtcNow;
            var toDate = frDate.AddDays(2);
            var usersAboutToExpire = await _context.Users
                .AsNoTracking()
                .Where(p => p.PremiumDate != null && p.PremiumDate > frDate && p.PremiumDate <= toDate)
                .Select(p => new RemindData
                {
                    ExpiredDate = p.PremiumDate.Value,
                    UserId = p.Id
                })
                .ToListAsync();

            await SendNotificationAsync(new RemindExpiredSubscriptionR
            {
                RemindDatas = usersAboutToExpire
            });
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    public async Task<bool> SendNotificationAsync(RemindExpiredSubscriptionR req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/RemindExpiredSubscription");

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
