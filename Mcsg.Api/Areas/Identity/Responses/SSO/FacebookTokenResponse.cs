using System.Security.Claims;

namespace Mcsg.Api.Areas.Identity.Responses;

public class FacebookTokenResponse
{
    public FacebookProfileModel Profile { get; set; }
    public List<Claim> Claims { get; set; }
    public List<string> Error { get; set; }
    public bool IsExists { get; set; }
}

public class FacebookProfileModel
{
    public string Id { get; set; }
    public string First_name { get; set; }
    public string Last_name { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
