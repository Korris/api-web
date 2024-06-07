#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using System.Net;
using System.Net.Mail;

namespace Mcsg.Common.Core.Notifications;

using Dtos;
using SeedWork.Responses;

/// <summary>
/// Notification SMTP
/// </summary>
public class NotificationSmtp : NotificationStrategy
{
    #region -- Overrides --

    /// <summary>
    /// Send
    /// </summary>
    /// <param name="inf">Email information</param>
    /// <returns>Return the result</returns>
    public override async Task<SingleResponse> Send(NotificationInfoDto inf)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        var res = new SingleResponse();
        var cred = new NetworkCredential(_auth.UserName, _auth.Password);

        var smtp = new SmtpClient(_auth.Host, _auth.Port)
        {
            Credentials = cred,
            EnableSsl = true
        };

        var msg = new MailMessage
        {
            IsBodyHtml = true
        };

        msg.To.Add(inf.To!);
        msg.From = new MailAddress(_auth.SenderEmail, _auth.SenderName);
        msg.Subject = inf.Subject;
        msg.Body = inf.Body;

        await smtp.SendMailAsync(msg);
        smtp.Dispose();

        return res;
    }

    #endregion
}
