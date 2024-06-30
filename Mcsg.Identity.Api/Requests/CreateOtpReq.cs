namespace Mcsg.Identity.Api.Requests;

using Lib.Data.Enums;

public class CreateOtpReq : BaseUserReq
{
    public UserOtpType Type { get; set; }
}
