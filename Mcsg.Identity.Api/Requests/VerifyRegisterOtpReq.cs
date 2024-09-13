namespace Mcsg.Identity.Api.Requests;

using Common.Core.Enums;

public class VerifyRegisterOtpReq : AuthenticationFormBaseR
{
    public UserOtpType Type { get; set; }
    public string Otp { get; set; }
    public string OtpToken { get; set; }
}
