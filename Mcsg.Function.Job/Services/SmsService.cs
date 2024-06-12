using Newtonsoft.Json;

namespace Mcsg.Function.Job.Services
{
    using Extensions;
    using Interfaces;
    using Lib.Common.Helpers;
    using Lib.Common.Models;
    using Entities = Lib.Data.Domain.Entities;

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
