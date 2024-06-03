using Mcsg.Function.Job.Extensions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Common.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(Entities.Job job);
    }
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;

        }

        public async Task SendSmsAsync(Entities.Job job)
        {
            var sms = JsonConvert.DeserializeObject<Sms>(job.Data);

            var result = SmsHelper.SendSms(sms.To, sms.Body.RenderSmsOtpBody());

            _logger.LogInformation($"SMS Service - To: {sms.To} - Body : {sms.Body.RenderSmsOtpBody()} - Result : {result}");
        }
    }
}
