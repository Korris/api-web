namespace Mcsg.Identity.Api.Requests;

using Common.Core.Enums;

public class CreateOtpReq : BaseUserReq
{
    public UserOtpType Type { get; set; }
}
