namespace Mcsg.Identity.Api.Requests;

using Common.Core.Enums;

using Common.Core.Requests;

public class ResendOtpReq : BaseR
{
    public UserOtpType Type { get; set; }
    public string OtpToken { get; set; }
}
