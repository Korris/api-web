using Newtonsoft.Json;

namespace Mcsg.Function.Job.Services;

using Common.Domain.Entities;
using Extensions;
using Interfaces;
using Lib.Common.Helpers;
using Lib.Common.Models;

public class SmsService : ISmsService
{
    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;

    }

    public async Task SendSmsAsync(Job job)
    {
        var sms = JsonConvert.DeserializeObject<Sms>(job.Data);

        var result = SmsHelper.SendSms(sms.To, sms.Body.RenderSmsOtpBody());

        _logger.LogInformation($"SMS Service - To: {sms.To} - Body : {sms.Body.RenderSmsOtpBody()} - Result : {result}");
    }

    #region -- Fields --

    private readonly ILogger<SmsService> _logger;

    #endregion
}
