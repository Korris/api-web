namespace Mcsg.Identity.Api.Requests;

using Common.Core.Enums;

public class AuthenticationResendOtpR : AuthenticationFormBaseR
{
    public UserOtpType Type { get; set; }
    public string OtpToken { get; set; }
}
