using System.Security.Claims;

namespace Mcsg.Api.Areas.Identity.Responses;

public class GoogleTokenResponse
{
    public GoogleProfileModel Profile { get; set; }
    public List<Claim> Claims { get; set; }
    public List<string> Error { get; set; }
    public bool IsExists { get; set; }
}

public class GoogleProfileModel
{
    public string Id { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Family_name { get; set; }
    public string Given_name { get; set; }
}
