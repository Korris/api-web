namespace Mcsg.Identity.Api.DTOs.Request
{
    public class RegisterUserReq : BaseUserReq
    {
        public string? ReferralCode { get; set; }
    }
}
