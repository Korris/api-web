namespace Mcsg.Identity.Api.Requests;

using Lib.Data.Enums;

public class ResendOtpReq
{
    public UserOtpType Type { get; set; }
    public string OtpToken { get; set; }
}
