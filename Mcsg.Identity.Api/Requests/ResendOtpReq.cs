namespace Mcsg.Identity.Api.Requests;

using Common.Core.Enums;

public class ResendOtpReq : BaseUserReq
{
    public UserOtpType Type { get; set; }
    public string OtpToken { get; set; }
}
