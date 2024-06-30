namespace Mcsg.Identity.Api.Requests;

using Attributes;

public class BaseUserReq
{
    [RequireEmailOrPhone]
    public string? Email { get; set; }

    [RequireEmailOrPhone]
    public string? Phone { get; set; }
}
