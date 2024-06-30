namespace Mcsg.Identity.Api.Requests;

public class CreateNewUserPasswordReq : BaseUserReq
{
    public string Otp { get; set; }
    public string OtpToken { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
