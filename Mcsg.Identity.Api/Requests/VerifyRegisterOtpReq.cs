namespace Mcsg.Identity.Api.Requests;

using Lib.Data.Enums;

public class VerifyRegisterOtpReq : BaseUserReq
{
    public UserOtpType Type { get; set; }
    public string Otp { get; set; }
    public string OtpToken { get; set; }
}
