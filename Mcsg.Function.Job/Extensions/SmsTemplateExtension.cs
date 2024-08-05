namespace Mcsg.Function.Job.Extensions;

public static class SmsTemplateExtension
{
    public static string RenderSmsOtpBody(this string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException();

        return $"OTP code is {message} ";
    }
}
