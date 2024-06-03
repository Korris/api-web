using Mcsg.Function.Job.Constants;
using Mcsg.Lib.Common.Models;
using System.Text;

namespace Mcsg.Function.Job.Extensions
{
    internal static class EmailTemplateExtension
    {
        internal static string RenderEmailOtpBody(this StringBuilder template, Email email)
        {
            if (email == null)
                throw new ArgumentNullException();

            return template
                .Replace("{{LOGO_URL}}", FunctionConstant.LogoUrl)
                .Replace("{{TO}}", email.To)
                .Replace("{{OTP}}", email.Body).ToString();
        }
        internal static string RenderEmailNotiAction(this StringBuilder template, Email email)
        {
            if (email == null)
                throw new ArgumentNullException();

            var listParams = email.Body.Split(',');

            return template
                .Replace("{{LOGO_URL}}", FunctionConstant.LogoUrl)
                .Replace("{{TO}}", email.To)
                .Replace("{{ACTION}}", listParams[0]).ToString()
                .Replace("{{FROM}}", listParams[1])
                .Replace("{{ID}}", listParams[2]);
        }
    }
}
