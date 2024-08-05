using Newtonsoft.Json;

namespace Mcsg.Function.Job.Services;

using Common.Domain.Entities;
using Common.SeedWork;
using Extensions;
using Interfaces;
using Lib.Common.Helpers;
using Lib.Common.Models;

public class SmsService : ISmsService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="aes"></param>
    /// <param name="logger"></param>
    public SmsService(ISecurityAes aes, ILogger<SmsService> logger)
    {
        _aes = aes;
        _logger = logger;
    }

    public async Task SendSmsAsync(Job job)
    {
        if (job.Data == null)
        {
            return;
        }

        var sms = JsonConvert.DeserializeObject<Sms>(job.Data);

        if (sms == null)
        {
            return;
        }

        sms.To = _aes.DecryptText(sms.To) + "";

        var result = SmsHelper.SendSms(sms.To, sms.Body.RenderSmsOtpBody());

        _logger.LogInformation($"SMS Service - To: {sms.To} - Body : {sms.Body.RenderSmsOtpBody()} - Result : {result}");
    }

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    private readonly ILogger<SmsService> _logger;

    #endregion
}
