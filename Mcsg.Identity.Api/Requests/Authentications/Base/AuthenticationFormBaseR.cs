namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class AuthenticationFormBaseR : BaseR
{
    public string? Email { get; set; }

    public string? Phone { get; set; }
}
