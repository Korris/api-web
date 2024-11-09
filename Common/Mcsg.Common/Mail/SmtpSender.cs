using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Mcsg.Lib.Common.Mail;

using Models;

public class SmtpSender : IEmailSender
{
    public SmtpSettings _smtpSetting { get; }

    public SmtpSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSetting = smtpSettings.Value;
    }

    public async Task<bool> SendEmail(Email email)
    {
        try
        {
            var client = new SmtpClient(_smtpSetting.SmtpHost, _smtpSetting.SmtpPort)
            {
                Credentials = new NetworkCredential(_smtpSetting.SmtpUser, _smtpSetting.SmtpPass),
                EnableSsl = true
            };

            client.Send(_smtpSetting.SmtpFrom, email.To, email.Subject, email.Body);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
        return true;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        try
        {
            var client = new SmtpClient(_smtpSetting.SmtpHost, _smtpSetting.SmtpPort)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSetting.SmtpUser, _smtpSetting.SmtpPass)
            };

            var fr = new MailAddress(_smtpSetting.SmtpFrom, _smtpSetting.SmtpDisplayFrom);
            var to = new MailAddress(toEmail);

            var email = new MailMessage(fr, to)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = message
            };

            #region -- Add BCC for testing purposes only --
            var bcc = "";
            if (toEmail.Contains("dev.teamsgsite.com"))
            {
                bcc = "ntada.bumcheo.dev@gmail.com";
            }
            else if (toEmail.Contains("uat.teamsgsite.com"))
            {
                bcc = "ntada.bumcheo.uat@gmail.com";
            }
            else if (toEmail.Contains("teamsgsite.com"))
            {
                bcc = "ntada.bumcheo.chung@gmail.com";
            }
            if (!string.IsNullOrEmpty(bcc))
            {
                email.Bcc.Add(bcc);
            }
            #endregion

            await client.SendMailAsync(email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return;
    }
}
