namespace Mcsg.Identity.Api.DTOs.Request
{
    public class SetUserPasswordReq
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
