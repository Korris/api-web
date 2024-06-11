using Microsoft.AspNetCore.Identity.UI.Services;
using Minio;
using Minio.DataModel.Args;
using Newtonsoft.Json;
using System.Text;

namespace Mcsg.Function.Job.Services
{
    using Constants;
    using Extensions;
    using Interfaces;
    using Lib.Common.Models;
    using Lib.Data.Enums;
    using Entities = Lib.Data.Domain.Entities;

    public class EmailService : IEmailService
    {
        public EmailService(IEmailSender emailSender, ISetting setting)
        {
            _emailSender = emailSender;
            _setting = setting;

            var minio = setting.Minio;
            _mc = new MinioClient().WithEndpoint(minio.EndPoint).WithCredentials(minio.AccessKey, minio.SecrectKey).WithRegion(minio.Location).Build();
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
            var file = $"email-templates/" + templateName;
            var ms = await GetObject(file);
            return new StringBuilder(MemoryStreamToString(ms));
        }

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="objectName">Object name (include full path and file extension)</param>
        /// <returns>Return the result</returns>
        private async Task<MemoryStream> GetObject(string objectName)
        {
            var res = new MemoryStream();

            if (string.IsNullOrWhiteSpace(objectName))
            {
                return res;
            }

            var minio = _setting.Minio;

            var statArg = new StatObjectArgs().WithBucket(minio.BucketName).WithObject(objectName);
            await _mc.StatObjectAsync(statArg);

            var getArg = new GetObjectArgs().WithBucket(minio.BucketName).WithObject(objectName).WithCallbackStream(p => { p.CopyTo(res); });
            await _mc.GetObjectAsync(getArg);

            return res;
        }

        static string MemoryStreamToString(MemoryStream memoryStream)
        {
            const int bufferSize = 1024; // 1 KB buffer size
            byte[] buffer = new byte[bufferSize];
            StringBuilder stringBuilder = new StringBuilder();

            // Ensure the position is at the beginning of the MemoryStream
            memoryStream.Position = 0;

            int bytesRead;
            while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                // Convert the read bytes to a string and append to the StringBuilder
                stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }

            return stringBuilder.ToString();
        }

        #region -- Fields --

        /// <summary>
        /// Email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        /// <summary>
        /// Minio client
        /// </summary>
        private readonly IMinioClient _mc;

        #endregion
    }
}
