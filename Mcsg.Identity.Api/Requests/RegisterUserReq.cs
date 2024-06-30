namespace Mcsg.Identity.Api.Requests;

public class RegisterUserReq : BaseUserReq
{
    public string? ReferralCode { get; set; }
}
