namespace Mcsg.Identity.Api.DTOs.Response
{
    public class VerifyUserResponse
    {
        public string Token { get; set; }
        public bool IsEmail { get; set; }
        public bool IsPhone { get; set; }
    }
}
