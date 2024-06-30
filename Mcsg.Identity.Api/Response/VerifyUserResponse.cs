namespace Mcsg.Identity.Api.Response;

public class VerifyUserResponse
{
    public string? Token { get; set; }
    public string? Code { get; set; }
    public string? UserName { get; set; }
    public bool IsEmail { get; set; }
    public bool IsPhone { get; set; }
}
