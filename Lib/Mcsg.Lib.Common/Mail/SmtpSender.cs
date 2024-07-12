using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Mcsg.Lib.Common.Mail;

using Models;

public class SmtpSender : IEmailSender
{
    public SmtpSettings _sendMailSettings { get; }

    public SmtpSender(IOptions<SmtpSettings> smtpSettings)
    {
        _sendMailSettings = smtpSettings.Value;
    }

    public async Task<bool> SendEmail(Email email)
    {
        try
        {
            var client = new SmtpClient(_sendMailSettings.SmtpHost, _sendMailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_sendMailSettings.SmtpUser, _sendMailSettings.SmtpPass),
                EnableSsl = true
            };
            client.Send(_sendMailSettings.SmtpFrom,
                email.To, email.Subject, email.Body);
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
            var client = new SmtpClient(_sendMailSettings.SmtpHost, _sendMailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_sendMailSettings.SmtpUser, _sendMailSettings.SmtpPass),
                EnableSsl = true,
            };
            Console.WriteLine($"Send mail from {_sendMailSettings.SmtpFrom} to {toEmail}");
            var email = new MailMessage(new MailAddress(_sendMailSettings.SmtpFrom, _sendMailSettings.SmtpDisplayFrom), new MailAddress(toEmail))
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
            throw e;
        }
        return;
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        //
    }
}
