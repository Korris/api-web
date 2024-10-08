using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mcsg.Wallet.Job.Services;

using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.SeedWork;
using Common.SeedWork.Enums;
using Common.SeedWork.Extensions;
using Constants;
using Domain.Entities;
using Extensions;
using Interfaces;
using Lib.Common.Models;

public class EmailService : IEmailService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="emailSender"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="aes"></param>
    public EmailService(IEmailSender emailSender, ISetting setting, IStorageClient sc, ISecurityAes aes)
    {
        _emailSender = emailSender;
        _setting = setting;
        _sc = sc;
        _aes = aes;
    }

    public async Task SendEmailAsync(Job job)
    {
        if (job.Data == null)
        {
            return;
        }

        var email = JsonConvert.DeserializeObject<Email>(job.Data);

        if (email == null)
        {
            return;
        }

        StringBuilder template;

        email.To = _aes.DecryptText(email.To) + "";

        switch (job.JobType)
        {
            case JobType.VerifyByEmailOtp:
                template = await DownloadEmailTemplateAsync("verify-email-otp.html");
                email.Subject = _setting.Information + FunctionConstant.OtpEmailTitle;
                email.Body = template.RenderEmailOtpBody(email);

                break;
            case JobType.ResetByEmailOtp:
                template = await DownloadEmailTemplateAsync("reset-email-otp.html");
                email.Subject = _setting.Information + FunctionConstant.OtpEmailTitle;
                email.Body = template.RenderEmailOtpBody(email);
                break;

            case JobType.ConfirmEmailOtp:
                template = await DownloadEmailTemplateAsync("common-email-otp.html");
                email.Subject = _setting.Information + FunctionConstant.OtpEmailTitle;
                email.Body = template.RenderEmailOtpBody(email);
                break;

            case JobType.WithDrawNoti:
                template = await DownloadEmailTemplateAsync("withdraw-noti.html");
                email.Subject = _setting.Information + FunctionConstant.WithdrawEmailTitle;
                email.Body = template.RenderEmailNotiAction(email);
                break;

            case JobType.DepositNoti:
                template = await DownloadEmailTemplateAsync("deposit-noti.html");
                email.Subject = _setting.Information + FunctionConstant.DepositEmailTitle;
                email.Body = template.RenderEmailNotiAction(email);
                break;

            default:
                throw new NotSupportedException();
        }

        await _emailSender.SendEmailAsync(email.To, email.Subject, email.Body);
    }

    private async Task<StringBuilder> DownloadEmailTemplateAsync(string templateName)
    {
        var file = $"email-templates/{templateName}";
        var ms = await _sc.GetStrategy(MinioInstanceType.Default).GetObject(file, null);
        return new StringBuilder(StreamExtension.ToString(ms));
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
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    #endregion
}
