using System.Text;

namespace Mcsg.Wallet.Job.Extensions;

using Lib.Common.Models;

internal static class EmailTemplateExtension
{
    internal static string RenderEmailOtpBody(this StringBuilder template, Email email)
    {
        if (email == null)
            throw new ArgumentNullException();

        return template
            .Replace("{{TO}}", email.To)
            .Replace("{{OTP}}", email.Body).ToString();
    }
    internal static string RenderEmailNotiAction(this StringBuilder template, Email email)
    {
        if (email == null)
            throw new ArgumentNullException();

        var listParams = email.Body.Split(',');

        return template
            .Replace("{{TO}}", email.To)
            .Replace("{{ACTION}}", listParams[0]).ToString()
            .Replace("{{FROM}}", listParams[1])
            .Replace("{{ID}}", listParams[2]);
    }
}
