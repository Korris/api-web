namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class BaseUserReq : BaseR
{
    public string? Email { get; set; }

    public string? Phone { get; set; }
}
