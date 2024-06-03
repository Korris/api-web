using Mcsg.Function.Job.Constants;
using Mcsg.Function.Job.Extensions;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Enums;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using System.Text;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(Entities.Job job);
    }

    public class EmailService : IEmailService
    {
        private readonly IAzureBlobStorageService _blobStorageService;
        private readonly IAzureBlobStorageService _publicBlobStorageService;
        private readonly IEmailSender _emailSender;
        public EmailService(IAzureBlobStorageService blobStorageService,
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _blobStorageService = blobStorageService;
            _publicBlobStorageService = new AzureBlobStorageService(Environment.GetEnvironmentVariable(FunctionConstant.PublicStorageConnection));
        }

        public async Task SendEmailAsync(Entities.Job job)
        {
            var email = JsonConvert.DeserializeObject<Email>(job.Data);
            _ = new StringBuilder();
            StringBuilder template;
            switch (job.JobType)
            {
                case JobType.VerifyByEmailOtp:
                    template = await DownloadEmailTemplateAsync("verify-email-otp.html");
                    email.Subject = FunctionConstant.OtpEmailTitle;
                    email.Body = template.RenderEmailOtpBody(email);

                    break;
                case JobType.ResetByEmailOtp:
                    template = await DownloadEmailTemplateAsync("reset-email-otp.html");
                    email.Subject = FunctionConstant.OtpEmailTitle;
                    email.Body = template.RenderEmailOtpBody(email);
                    break;
                case JobType.ConfirmEmailOtp:
                    template = await DownloadEmailTemplateAsync("common-email-otp.html");
                    email.Subject = FunctionConstant.OtpEmailTitle;
                    email.Body = template.RenderEmailOtpBody(email);
                    break;
                case JobType.WithDrawNoti:
                    template = await DownloadEmailTemplateAsync("withdraw-noti.html");
                    email.Subject = FunctionConstant.WithdrawEmailTitle;
                    email.Body = template.RenderEmailNotiAction(email);
                    break;
                case JobType.DepositNoti:
                    template = await DownloadEmailTemplateAsync("deposit-noti.html");
                    email.Subject = FunctionConstant.DepositEmailTitle;
                    email.Body = template.RenderEmailNotiAction(email);
                    break;
                default:
                    throw new NotSupportedException();
            }

            await _emailSender.SendEmailAsync(email.To, email.Subject, email.Body);
        }


        private async Task<StringBuilder> DownloadEmailTemplateAsync(string templateName)
        {
            await using var templateStream = await _publicBlobStorageService.DownloadAsync(templateName, "email-templates");
            using StreamReader reader = new(templateStream);
            return new StringBuilder(await reader.ReadToEndAsync());
        }
    }
}
