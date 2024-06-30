namespace Mcsg.Identity.Api.Requests;

public class SetUserPasswordReq
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
