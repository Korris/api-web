namespace Mcsg.Identity.Api.Response;

public class VerifyUserResponse
{
    public string? Token { get; set; }
    public bool IsEmail { get; set; }
    public bool IsPhone { get; set; }

    /// <summary>
    /// OTP code for DevMode
    /// </summary>
    public string? OtpCode { get; set; }

    /// <summary>
    /// UserName for DevMode
    /// </summary>
    public string? UserName { get; set; }
}
