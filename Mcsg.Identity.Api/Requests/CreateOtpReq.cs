namespace Mcsg.Identity.Api.Requests;

using Common.Core.Enums;

public class CreateOtpReq : AuthenticationFormBaseR
{
    public UserOtpType Type { get; set; }
}
